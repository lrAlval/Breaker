using System;

namespace Breaker.Events
{
    public class CircuitBreakerClosed
    {
        public int SuccessCount { get; set; }
        public TimeSpan MeanOperationTime { get; set; }
    }
}