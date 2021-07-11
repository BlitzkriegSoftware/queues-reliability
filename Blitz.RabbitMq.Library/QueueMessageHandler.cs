using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Blitz.RabbitMq.Library
{
    /// <summary>
    /// Delegate Queue item handler
    /// </summary>
    /// <param name="queueEngine">IQueueEngine</param>
    /// <param name="logger">ILogger</param>
    /// <param name="model">IModel</param>
    /// <param name="ea">BasicDeliverEventArgs</param>
    public delegate void QueueMessageHandler(IQueueEngine queueEngine, ILogger logger, IModel model, BasicDeliverEventArgs ea);
}
