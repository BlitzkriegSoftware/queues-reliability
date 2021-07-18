using Blitz.Reliability.Demo.Models;
using System;
using System.Collections.Generic;

namespace Blitz.Reliability.Demo.Transactions
{
    /// <summary>
    /// Unit of Work
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Tracking
        /// </summary>
        public ReliabilityTracking Tracking { get; set; }
        /// <summary>
        /// Unit of Work Data
        /// </summary>
        public Dictionary<string, object> UnitOfWorkData { get; set; }

        private readonly BlitzkriegSoftware.SecureRandomLibrary.SecureRandom dice = new BlitzkriegSoftware.SecureRandomLibrary.SecureRandom();

        /// <summary>
        /// Factory to make units of work
        /// </summary>
        /// <returns>A Unit of Work</returns>
        public UnitOfWork MakeUnitOfWork()
        {
            int MaxData = dice.Next(5, 11);
            var uow = new UnitOfWork()
            {
                Id = Guid.NewGuid(),
                Tracking = new ReliabilityTracking()
                {
                    Error = null,
                    LastOperationStamp = DateTime.UtcNow,
                    Retries = 0,
                    Status = UnitOfWorkStatus.New
                },
                UnitOfWorkData = new Dictionary<string, object>()
            };

            for (int i = 0; i < MaxData; i++)
            {
                uow.UnitOfWorkData.Add(Convert.ToChar(65 + i).ToString(), Faker.Lorem.Sentence(dice.Next(3, 9)));
            }

            return uow;
        }

    }
}

