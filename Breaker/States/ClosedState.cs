using System;
using System.Threading;

namespace Breaker.Core.States
{
    public class ClosedState : CircuitBreakerState
    {
        private readonly int _maxFailures;
        
        public ClosedState(CircuitBreaker circuitBreaker) : base(circuitBreaker) => _maxFailures = CircuitBreaker.Settings.FailuresThreshold;

        public override void OnEnter() => CircuitBreaker.FailureCount = 0;

        public override void InvocationSucceeds() => CircuitBreaker.FailureCount = 0;

        public override void InvocationFails(Exception e)
        {
            if (FailureThresholdReached())
            {
                CircuitBreaker.TripTo(new OpenState(CircuitBreaker));
            }
        }

        private bool FailureThresholdReached() => Interlocked.Increment(ref CircuitBreaker.FailureCount) == _maxFailures;
    }
}