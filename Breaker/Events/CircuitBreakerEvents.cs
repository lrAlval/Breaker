using System;
using Breaker.Core;

namespace Breaker.Events
{
    public class CircuitBreakerEvents
    {
        /// <summary>
        /// Raised when the circuit breaker enters the closed state
        /// </summary>
        public event Action<CircuitBreakerOpened> ClosedCircuitBreaker;

        /// <summary>
        /// Raised when the circuit breaker enters the opened state
        /// </summary>
        public event Action<CircuitBreakerOpened> OpenedCircuitBreaker;

        /// <summary>
        /// Raised when the circuit breaker enters the Half open state
        /// </summary>
        public event Action<CircuitBreakerHalfOpen> HalfOpenedCircuitBreaker;

        /// <summary>
        /// Raised when the circuit breaker changes state
        /// </summary>
        public event Action<CircuitBreakerState> OnStateChange;

        public void NotifyStateChange(CircuitBreakerState state) => OnStateChange?.Invoke(state);
    }
}