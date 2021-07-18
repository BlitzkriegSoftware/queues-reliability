using System;

namespace Blitz.Reliability.Demo.Models
{
    /// <summary>
    /// Reliability Tracking
    /// </summary>
    public class ReliabilityTracking
    {
        /// <summary>
        /// Retres
        /// </summary>
        public int Retries { get; set; } = 0;
        /// <summary>
        /// Last Status
        /// </summary>
        public UnitOfWorkStatus Status { get; set; } = UnitOfWorkStatus.New;
        /// <summary>
        /// Error (Exception)
        /// </summary>
        public Exception Error { get; set; } = null;

        /// <summary>
        /// Time Stamp
        /// </summary>
        public DateTime LastOperationStamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Debugging string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{LastOperationStamp:o}: {Status}, Retries: {Retries}, Ex: {Error.Message}";
        }
    }
}
