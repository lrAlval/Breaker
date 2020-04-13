using System;
using Breaker.Core;

namespace Breaker.Events
{
    public class CircuitBreakerEvents
    {
        /// <summary>
        /// Raised when the circuit breaker changes state
        /// </summary>
        public event Action<CircuitBreakerState> OnStateChange;

        public void NotifyStateChange(CircuitBreakerState state) => OnStateChange?.Invoke(state);
    }
}