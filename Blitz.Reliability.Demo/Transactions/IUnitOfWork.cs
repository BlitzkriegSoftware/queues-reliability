using Blitz.Reliability.Demo.Models;
using System;
using System.Collections.Generic;

namespace Blitz.Reliability.Demo.Transactions
{
    /// <summary>
    /// Unit of work
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Unit of Work Identifier
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Reliability Component
        /// </summary>
        ReliabilityTracking Tracking { get; set; }

        /// <summary>
        /// Unit of Work Data
        /// </summary>
        Dictionary<string, object> UnitOfWorkData { get; set; }

        /// <summary>
        /// Make a Unit of Work (for testing)
        /// </summary>
        /// <returns></returns>
        UnitOfWork MakeUnitOfWork();
    }
}
