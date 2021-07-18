using Blitz.Reliability.Demo.Extensions;
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
        public long Id { get; set; }
        /// <summary>
        /// Tracking
        /// </summary>
        public ReliabilityTracking Tracking { get; set; }
        /// <summary>
        /// Unit of Work Data
        /// </summary>
        public Dictionary<string, object> UnitOfWorkData { get; set; }

        /// <summary>
        /// Factory to make units of work
        /// </summary>
        /// <returns>A Unit of Work</returns>
        public static UnitOfWork MakeUnitOfWork(long id)
        {
            var dice = new BlitzkriegSoftware.SecureRandomLibrary.SecureRandom();
            int MaxData = dice.Next(1, 3);
            var uow = new UnitOfWork()
            {
                Id = id,
                Tracking = new ReliabilityTracking()
                {
                    Error = null,
                    LastOperationStamp = DateTime.UtcNow,
                    Tries = 0,
                    Status = UnitOfWorkStatus.New
                },
                UnitOfWorkData = new Dictionary<string, object>()
            };

            for (int i = 0; i < MaxData; i++)
            {
                uow.UnitOfWorkData.Add(Convert.ToChar(65 + i).ToString(), Faker.Lorem.Words(dice.Next(1, 3)));
            }

            return uow;
        }

        /// <summary>
        /// Debug string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Id: {Id}, RT: {Tracking}, Data: {UnitOfWorkData.ToJson()}";
        }

    }
}

