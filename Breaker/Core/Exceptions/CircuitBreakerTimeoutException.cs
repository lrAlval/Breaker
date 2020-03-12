using System;

namespace Breaker.Core
{
    public class CircuitBreakerTimeoutException : Exception
    {
        public CircuitBreakerTimeoutException(string message = "The operation has timed out.") : base(message) { }
    }
}