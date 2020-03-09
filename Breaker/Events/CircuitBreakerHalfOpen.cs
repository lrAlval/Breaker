using System;
using System.Collections.Generic;
using System.Text;

namespace Breaker.Events
{
    public class CircuitBreakerHalfOpen
    {
        public TimeSpan TimeoutCompleted { get; set; }
        public int FailureThreshold { get; set; }
    }
}
