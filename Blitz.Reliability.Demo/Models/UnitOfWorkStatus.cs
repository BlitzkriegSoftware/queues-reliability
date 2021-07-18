using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz.Reliability.Demo.Models
{
    /// <summary>
    /// Enum: Unit of Work Status
    /// </summary>
    public enum UnitOfWorkStatus
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// New
        /// </summary>
        New = 1,
        /// <summary>
        /// Retried
        /// </summary>
        Retried = 2,
        /// <summary>
        /// Fatal Error
        /// </summary>
        Fatal = 3,
        /// <summary>
        /// Number of retries exceeded
        /// </summary>
        Expired = 4
    }
}
