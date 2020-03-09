using System;
using System.Collections.Generic;
using System.Text;

namespace Breaker.Events
{
    public class CircuitBreakerOpened
    {
        public Exception Reason { get; set; }
        public int Failures { get; set; }
        public TimeSpan FailureMean { get; set; }
    }
}