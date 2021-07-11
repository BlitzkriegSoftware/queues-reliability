using System;
using RabbitMQ.Client;

namespace Blitz.RabbitMq.Library.Libs
{
    /// <summary>
    /// Utility: Useful RabbitMQ utilities
    /// </summary>
    public static class RabbitMqUtility
    {
        /// <summary>
        /// Make an RabbitMQ connection factory 
        /// </summary>
        /// <param name="hostname">(sic)</param>
        /// <param name="port">Use <c>RabbitMq_Port_Default</c></param>
        /// <param name="username">(sic)</param>
        /// <param name="password">(sic)</param>
        /// <returns>ConnectionFactory</returns>
        public static ConnectionFactory RabbitMQMakeConnectionFactory(string hostname, int port, string username, string password)
        {
            if (string.IsNullOrEmpty(hostname)) throw new ArgumentNullException(nameof(hostname));

            return new ConnectionFactory()
            {
                HostName = hostname,
                Port = port,
                UserName = username,
                Password = password
            };
        }

        /// <summary>
        /// Message Properties: Make some as persistant w. long expirations
        /// </summary>
        /// <param name="model">IModel</param>
        /// <param name="deliveryMode">Enum</param>
        /// <param name="persistent">bool</param>
        /// <param name="expiration">ms as string</param>
        /// <returns>IBasicProperties</returns>
        public static IBasicProperties MessageBasicPropertiesPersistant(IModel model, byte deliveryMode, bool persistent, string expiration)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(expiration)) throw new ArgumentNullException(nameof(expiration));

            var props = model.CreateBasicProperties();
            props.DeliveryMode = deliveryMode;
            props.Persistent = persistent;
            props.Expiration = expiration;
            return props;
        }


        /// <summary>
        /// Set up Durable Queue
        /// </summary>
        /// <param name="model">IModel</param>
        /// <param name="engineConfiguration">RabbitMqEngineConfiguration</param>
        /// <param name="queueConfiguration">RabbitMqInstanceConfiguration</param>
        public static void SetupDurableQueue(IModel model, Models.RabbitMqEngineConfiguration engineConfiguration, Models.RabbitMqInstanceConfiguration queueConfiguration)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (engineConfiguration == null) throw new ArgumentNullException(nameof(engineConfiguration));
            if (queueConfiguration == null) throw new ArgumentNullException(nameof(queueConfiguration));

            model.ExchangeDeclare(queueConfiguration.ExchangeName, ExchangeType.Direct);

            model.QueueDeclare(
                        queue: queueConfiguration.QueueName,
                        durable: engineConfiguration.QueueDurable,
                        exclusive: engineConfiguration.QueueExclusive,
                        autoDelete: engineConfiguration.QueueAutoDelete,
                        arguments: null);

            model.QueueBind(
                        queue: queueConfiguration.QueueName,
                        exchange: queueConfiguration.ExchangeName,
                        routingKey: queueConfiguration.RoutingKey,
                        arguments: null);
        }

        /// <summary>
        /// Message: Publish to queue and exchange set up by <c>SetupDurableQueue</c>
        /// </summary>
        /// <param name="model">IModel</param>
        /// <param name="exchangeName">(sic)</param>
        /// <param name="routingKey">(sic)</param>
        /// <param name="messageProperties">IBasicProperties</param>
        /// <param name="body">bytes</param>
        public static void Publish(IModel model, string exchangeName, string routingKey, IBasicProperties messageProperties, byte[] body)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(routingKey)) throw new ArgumentNullException(nameof(routingKey));
            if (messageProperties == null) throw new ArgumentNullException(nameof(messageProperties));
            if ((body == null) || (body.Length <= 0)) throw new ArgumentNullException(nameof(body));

            model.BasicPublish(
                               exchange: exchangeName,
                               routingKey: routingKey,
                               basicProperties: messageProperties,
                               body: body
                           );
        }

    }
}
