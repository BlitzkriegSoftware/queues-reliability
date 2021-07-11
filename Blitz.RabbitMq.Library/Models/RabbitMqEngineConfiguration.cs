using System;

namespace Blitz.RabbitMq.Library.Models
{
    /// <summary>
    /// Configuration: One RabbitMQ Instance
    /// </summary>
    public class RabbitMqEngineConfiguration
    {
        /// <summary>
        /// Localhost
        /// </summary>
        public const string Localhost_Default = "localhost";

        /// <summary>
        /// Username: guest
        /// </summary>
        public const string Username_Default = "guest";

        /// <summary>
        /// Password: guest
        /// </summary>
        public const string Password_Default = "guest";

        /// <summary>
        /// Port
        /// </summary>
        public const int Port_Default = 5672;

        /// <summary>
        /// Send Sleep Time (ms)
        /// </summary>
        public const int SendSleep_Default = 1000;

        /// <summary>
        /// Messages to send
        /// </summary>
        public const long Messages_To_Send = 200;

        /// <summary>
        /// Message Property: Expiration 600 minutes
        /// </summary>
        public const string Message_Expiration_Default = "36000000";

        /// <summary>
        /// Message Property: Persistant Message
        /// </summary>
        public const int Message_DeliveryMode_Default = 2;

        /// <summary>
        /// Message Property: Persistant Message
        /// </summary>
        public const bool Message_Persistent_Default = true;

        /// <summary>
        /// Queue Property: Persistant Queue
        /// </summary>
        public const bool Queue_Durable_Default = true;

        /// <summary>
        /// Queue Property: Not exclusive
        /// </summary>
        public const bool Queue_Exclusive_Default = false;

        /// <summary>
        /// Queue Property: Do not auto delete
        /// </summary>
        public const bool Queue_AutoDelete_Default = false;

        /// <summary>
        /// Host (default: <c>Localhost_Default</c>)
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port: (default: <c>Port_Default</c>)
        /// </summary>
        public int Port { get; set; }

        /// Username: (default: <c>Username_Default</c>)
        public string Username { get; set; }

        /// Password: (default: <c>Password_Default</c>)
        public string Password { get; set; }

        /// <summary>
        /// MessageExpiration (default: <c>Message_Expiration_Default</c>)
        /// <para>This is a long in milliseconds (ms) as a string</para>
        /// </summary>
        public string MessageExpiration { get; set; }

        /// <summary>
        /// MessageDeliveryMode (default: <c>Message_DeliveryMode_Default</c>)
        /// </summary>
        public byte MessageDeliveryMode { get; set; }

        /// <summary>
        /// MessagePersistent (default: <c>Message_Persistent_Default</c> aka true)
        /// </summary>
        public bool MessagePersistent { get; set; }

        /// <summary>
        /// QueueDurable (default: <c>Queue_Durable_Default</c> aka true)
        /// </summary>
        public bool QueueDurable { get; set; }

        /// <summary>
        ///   (default: <c>Queue_Exclusive_Default</c>) aka false
        /// </summary>
        public bool QueueExclusive { get; set; }

        /// <summary>
        /// QueueAutoDelete (default: <c>Queue_AutoDelete_Default</c> aka false
        /// </summary>
        public bool QueueAutoDelete { get; set; }

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
                case "host": this.Host = value; break;
                case "messagedeliverymode": this.MessageDeliveryMode = Convert.ToByte(value); break;
                case "messageexpiration": this.MessageExpiration = value; break;
                case "messagepersistent": this.MessagePersistent = Convert.ToBoolean(value); break;
                case "password": this.Password = value; break;
                case "port": this.Port = Convert.ToInt32(value); break;
                case "queueautodelete": this.QueueAutoDelete = Convert.ToBoolean(value); break;
                case "queuedurable": this.QueueDurable = Convert.ToBoolean(value); break;
                case "queueexclusive": this.QueueExclusive = Convert.ToBoolean(value); break;
                case "username": this.Username = value; break;
            }
        }

        /// <summary>
        /// Create Default Configuration
        /// </summary>
        /// <returns>RabbitMqConfiguration</returns>
        public static RabbitMqEngineConfiguration CreateDefault()
        {
            var rc = new RabbitMqEngineConfiguration()
            {
                Host = RabbitMqEngineConfiguration.Localhost_Default,
                MessageDeliveryMode = RabbitMqEngineConfiguration.Message_DeliveryMode_Default,
                MessageExpiration = RabbitMqEngineConfiguration.Message_Expiration_Default,
                MessagePersistent = RabbitMqEngineConfiguration.Message_Persistent_Default,
                Password = RabbitMqEngineConfiguration.Password_Default,
                Port = RabbitMqEngineConfiguration.Port_Default,
                QueueAutoDelete = RabbitMqEngineConfiguration.Queue_AutoDelete_Default,
                QueueDurable = RabbitMqEngineConfiguration.Queue_Durable_Default,
                QueueExclusive = RabbitMqEngineConfiguration.Queue_Exclusive_Default,
                Username = RabbitMqEngineConfiguration.Username_Default
            };
            return rc;
        }

        /// <summary>
        /// Debugging string
        /// </summary>
        /// <returns>Debugging String</returns>
        public override string ToString()
        {
            return string.Format($"RabbitMQ - {this.Host}: {this.Port} as '{this.Username}', exp: {this.MessageExpiration} ms, Delivery Mode {this.MessageDeliveryMode}, Persist: {this.MessagePersistent}, AutoDelete: {this.QueueAutoDelete}, Durable: {this.QueueDurable}, Exclusive: {this.QueueExclusive}");
        }

    }
}
