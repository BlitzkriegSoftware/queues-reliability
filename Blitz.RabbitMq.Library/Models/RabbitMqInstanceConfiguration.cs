using System;

namespace Blitz.RabbitMq.Library.Models
{
    /// <summary>
    /// Configuration: Specific Queue
    /// </summary>
    public class RabbitMqInstanceConfiguration
    {
        /// <summary>
        /// Quick Name: Exchange
        /// </summary>
        public const string ExchangeName_Default = "myExchange";

        /// <summary>
        /// Quick Name: Queue
        /// </summary>
        public const string QueueName_Default = "myQueue";

        /// <summary>
        /// Quick Name: Routing Key
        /// </summary>
        public const string RoutingKey_Default = "myRoutingKey";

        /// <summary>
        /// (optional) Exchange
        /// </summary>
        public string ExchangeName { get; set; } = RabbitMqInstanceConfiguration.ExchangeName_Default;

        /// <summary>
        /// (required) Queue Name
        /// </summary>
        public string QueueName { get; set; } = RabbitMqInstanceConfiguration.QueueName_Default;

        /// <summary>
        /// (optional) Route
        /// </summary>
        public string RoutingKey { get; set; } = RabbitMqInstanceConfiguration.RoutingKey_Default;

        /// <summary>
        /// Is this valid?
        /// </summary>
        public bool isValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.QueueName);
            }
        }

        /// <summary>
        /// Set Property
        /// </summary>
        /// <param name="key">(sic)</param>
        /// <param name="value">(sic)</param>
        public void SetProperty(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            switch (key.ToLowerInvariant())
            {
                case "exchangename": this.ExchangeName = value; break;
                case "queuename": this.QueueName = value; break;
                case "routingkey": this.RoutingKey = value; break;
            }
        }

        /// <summary>
        /// To String
        /// </summary>
        /// <returns>Debug String</returns>
        public override string ToString()
        {
            return $"Exchange: {this.ExchangeName}, Queue: {this.QueueName}, Route: {this.RoutingKey}";
        }
    }
}
