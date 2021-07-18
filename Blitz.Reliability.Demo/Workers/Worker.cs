using Blitz.RabbitMq.Library;
using Blitz.RabbitMq.Library.Models;
using Blitz.Reliability.Demo.Models;
using Blitz.Reliability.Demo.Transactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Blitz.Reliability.Demo.Workers
{
    /// <summary>
    /// Worker
    /// </summary>
    public class Worker : IWorker
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _config;
        private RabbitMQClient client;
        private static readonly BlitzkriegSoftware.SecureRandomLibrary.SecureRandom dice = new();

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="config">IConfigurationRoot</param>
        public Worker(ILogger<Worker> logger, IConfigurationRoot config)
        {
            this._logger = logger;
            this._config = config;
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="o">CommandOptions</param>
        public void Run(CommandOptions o)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));

            var queueConfig = new RabbitMq.Library.Models.RabbitMqInstanceConfiguration();
            foreach (var c in this._config.AsEnumerable())
            {
                queueConfig.SetProperty(c.Key, c.Value);
            }

            this._logger.LogDebug(queueConfig.ToString());

            this.client = new RabbitMQClient(this._logger, this._config);

            this.client.PurgeQueue(queueConfig);

            for(int i=0; i<o.UnitOfWorkCount; i++)
            {
                var uow = Transactions.UnitOfWork.MakeUnitOfWork();
                this.client.Enqueue<Transactions.UnitOfWork>(uow, queueConfig);
            }

            int listenFor = o.UnitOfWorkCount * 10 + 5000;

            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(listenFor).ConfigureAwait(false);
                this.client.KeepListening = false;
            });

            this.client.SetupDequeueEvent(queueConfig, MyQueueMessageHandler);
        }

        /// <summary>
        /// Handler: Applies Random Unit of Work
        /// </summary>
        /// <param name="queueEngine">IQueueEngine</param>
        /// <param name="logger">ILogger</param>
        /// <param name="model">IModel</param>
        /// <param name="ea">BasicDeliverEventArgs</param>
        public static void MyQueueMessageHandler(
            IQueueEngine queueEngine, 
            ILogger logger, 
            IModel model, 
            BasicDeliverEventArgs ea)
        {
            if (queueEngine == null) throw new ArgumentNullException(nameof(queueEngine));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (ea == null) throw new ArgumentNullException(nameof(ea));

            // Convert To Unit of Work
            var body = ea.Body;
            var text = Encoding.UTF8.GetString(body.ToArray());
            var message = JsonConvert.DeserializeObject<Transactions.IUnitOfWork>(text);

            var state = ReceivedMessageState.SuccessfullyProcessed;

            // Simulate various conditions
            var chance = dice.Next(1, 100);
            switch(chance)
            {
                case < 10:
                    break;
                default: // success, dequeue the message as having been done
                    break;
            }

            logger.LogInformation("Received: {0}, Status: {1}", text, state);

            queueEngine.SendResponse(model, ea, state);
        }

    }
}
