using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz.Reliability.Demo.Workers
{
    interface IWorker
    {
        public void Run(Models.CommandOptions o);
    }
}
