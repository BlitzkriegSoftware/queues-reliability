using Blitz.RabbitMq.Library;
using Blitz.RabbitMq.Library.Models;
using Blitz.Reliability.Demo.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Blitz.Reliability.Demo.Workers
{
    /// <summary>
    /// Worker
    /// </summary>
    public class Worker : IWorker
    {
        #region "Vars, Consts, CTOR"

        private readonly ILogger _logger;
        private readonly IConfigurationRoot _config;
        private static readonly BlitzkriegSoftware.SecureRandomLibrary.SecureRandom dice = new();
        private static RabbitMqInstanceConfiguration RabbitConfig;
        private static RabbitMQClient RabbitClient;

        private const int UnitOfWorkMaxRetries = 3;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="config">IConfigurationRoot</param>
        public Worker(ILogger<Worker> logger, IConfigurationRoot config)
        {
            this._logger = logger;
            this._config = config;

            var queueConfig = new RabbitMq.Library.Models.RabbitMqInstanceConfiguration();
            foreach (var c in this._config.AsEnumerable())
            {
                queueConfig.SetProperty(c.Key, c.Value);
            }
            RabbitConfig = queueConfig;
        }

        #endregion

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="o">CommandOptions</param>
        public void Run(CommandOptions o)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));

            this._logger.LogDebug(RabbitConfig.ToString());

            RabbitClient = new RabbitMQClient(this._logger, this._config);

            RabbitClient.PurgeQueue(RabbitConfig);

            for (int i = 0; i < o.UnitOfWorkCount; i++)
            {
                var uow = Transactions.UnitOfWork.MakeUnitOfWork((i + 1));
                RabbitClient.Enqueue<Transactions.UnitOfWork>(uow, RabbitConfig);
            }

            int listenFor = o.UnitOfWorkCount * 150 + 5000;

            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(listenFor).ConfigureAwait(false);
                RabbitClient.KeepListening = false;
            });

            RabbitClient.SetupDequeueEvent(RabbitConfig, MyQueueMessageHandler);
        }

        /// <summary>
        /// Handler: Processes Unit of Work
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
            var message = JsonConvert.DeserializeObject<Transactions.UnitOfWork>(text);

#pragma warning disable IDE0059 // Make clear the initial state
            var state = ReceivedMessageState.SuccessfullyProcessed;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            message.Tracking.LastOperationStamp = DateTime.UtcNow;
            message.Tracking.Tries++;

            // Simulate various conditions
            var chance = dice.Next(1, 100);
            switch (chance)
            {

                case int i when (i < 10): // Process itself has throw a fatal exception, oh no
                    state = ReceivedMessageState.MessageRejected;
                    message.Tracking.Status = UnitOfWorkStatus.Fatal;
                    message.Tracking.Error = new InvalidOperationException("Fatal Horribleness");
                    break;

                case int i when ((i >= 10) && (i < 50)): // Need to Retry
                    if (message.Tracking.Tries >= UnitOfWorkMaxRetries)
                    {
                        state = ReceivedMessageState.MessageRejected;
                        message.Tracking.Status = UnitOfWorkStatus.Expired;
                        message.Tracking.Error = new InvalidOperationException("Retries Exceeded");
                    }
                    else
                    {
                        state = ReceivedMessageState.SuccessfullyProcessed;
                        message.Tracking.Status = UnitOfWorkStatus.Retried;
                        // Compensate by putting the unit of work into the future
                        int delay = (int)Math.Pow(2, (message.Tracking.Tries + 1)) * 500;
                        RabbitClient.Enqueue<Transactions.IUnitOfWork>(message, RabbitConfig, delay);
                    }
                    break;

                default: // success, dequeue the message as having been done
                    state = ReceivedMessageState.SuccessfullyProcessed;
                    message.Tracking.Status = UnitOfWorkStatus.Success;
                    break;
            }

            logger.LogInformation($"Msg: {message}, State: {state}");

            queueEngine.SendResponse(model, ea, state);
        }

    }
}
