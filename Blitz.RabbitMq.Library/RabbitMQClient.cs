using Blitz.RabbitMq.Library.Libs;
using Blitz.RabbitMq.Library.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Blitz.RabbitMq.Library
{
    /// <summary>
    /// Client: RabbitMQ Blitzkrieg Style
    /// </summary>
    public class RabbitMQClient : IQueueEngine
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _config;

        private readonly RabbitMqEngineConfiguration _engineConfiguration;

        // Default CTOR not allowed
        private RabbitMQClient() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="config">IConfigurationRoot</param>
        public RabbitMQClient(ILogger logger, IConfigurationRoot config)
        {
            this._logger = logger;
            this._config = config;

            this._engineConfiguration = RabbitMqEngineConfiguration.CreateDefault();
            foreach (var c in this._config.AsEnumerable())
            {
                this._engineConfiguration.SetProperty(c.Key, c.Value);
            }

            this._logger.LogDebug(this._engineConfiguration.ToString());
        }

        /// <summary>
        /// Keep Listening
        /// </summary>
        public bool KeepListening { get; set; } = true;

        /// <summary>
        /// Dequeue a message 
        /// </summary>
        /// <param name="queueConfiguration">(sic)</param>
        /// <param name="handler">QueueMessageHandler</param>
        public void SetupDequeueEvent(Models.RabbitMqInstanceConfiguration queueConfiguration, QueueMessageHandler handler)
        {
            var factory = RabbitMqUtility.RabbitMQMakeConnectionFactory(this._engineConfiguration.Host, this._engineConfiguration.Port, this._engineConfiguration.Username, this._engineConfiguration.Password);

            using (var connection = factory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    RabbitMqUtility.SetupDurableQueue(model, this._engineConfiguration, queueConfiguration);

                    var consumer = new EventingBasicConsumer(model);

                    consumer.Received += (_, ea) =>
                    {
                        handler(this, this._logger, model, ea);
                    };

                    model.BasicConsume(queue: queueConfiguration.QueueName,
                                         autoAck: false,
                                         consumer: consumer);

                    while (this.KeepListening) { }
                }
            }
        }


        /// <summary>
        /// Enqueue message
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="message">Message of T</param>
        /// <param name="queueConfiguration">RabbitMqInstanceConfiguration</param>
        public void Enqueue<T>(T message, Models.RabbitMqInstanceConfiguration queueConfiguration)
        {
            var factory = RabbitMqUtility.RabbitMQMakeConnectionFactory(this._engineConfiguration.Host, this._engineConfiguration.Port, this._engineConfiguration.Username, this._engineConfiguration.Password);

            using (var connection = factory.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    RabbitMqUtility.SetupDurableQueue(model, this._engineConfiguration, queueConfiguration);

                    var messageProperties = RabbitMqUtility.MessageBasicPropertiesPersistant(model, this._engineConfiguration.MessageDeliveryMode, this._engineConfiguration.MessagePersistent, this._engineConfiguration.MessageExpiration);

                    // Make message
                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    // Send message
                    RabbitMqUtility.Publish(model, queueConfiguration.ExchangeName, queueConfiguration.RoutingKey, messageProperties, body);

                    this._logger.LogInformation("Published: {0}", json);
                }
            }
        }

        /// <summary>
        /// Ack/Nack/Reject Message (must be called by the <c>QueueMessageHandler</c>
        /// </summary>
        /// <param name="model">IModel</param>
        /// <param name="ea">BasicDeliverEventArgs</param>
        /// <param name="state">ReceivedMessageState</param>
        public void SendResponse(IModel model, BasicDeliverEventArgs ea, ReceivedMessageState state)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (ea == null) throw new ArgumentNullException(nameof(ea));

            switch (state)
            {
                case ReceivedMessageState.SuccessfullyProcessed:
                    // Success remove from queue
                    model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    break;
                case ReceivedMessageState.UnsuccessfulProcessing:
                    // Unsuccessful, requeue and retry
                    model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    break;
                default:
                    // Bad Message, Reject and Delete
                    model.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
                    break;
            }
        }

        /// <summary>
        /// Delete all message in a queue (Purge)
        /// </summary>
        /// <param name="queueConfiguration">QueueInstanceConfiguration</param>
        public void PurgeQueue(Models.RabbitMqInstanceConfiguration queueConfiguration)
        {
            if (queueConfiguration == null) throw new ArgumentNullException(nameof(queueConfiguration));

            var factory = Libs.RabbitMqUtility.RabbitMQMakeConnectionFactory(this._engineConfiguration.Host, this._engineConfiguration.Port, this._engineConfiguration.Username, this._engineConfiguration.Password);

            using (var connection = factory.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    try
                    {
                        model.QueuePurge(queueConfiguration.QueueName);
                    }
                    catch { }
                }
            }
        }

    }
}
