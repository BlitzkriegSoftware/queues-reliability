namespace Blitz.RabbitMq.Library.Models
{
    /// <summary>
    /// What is the state of the received message
    /// </summary>
    public enum ReceivedMessageState : int
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Success
        /// </summary>
        SuccessfullyProcessed = 1,
        /// <summary>
        /// Unsuccessful
        /// </summary>
        UnsuccessfulProcessing = 2,
        /// <summary>
        /// Bad message, rejected
        /// </summary>
        MessageRejected = 3
    }
}
