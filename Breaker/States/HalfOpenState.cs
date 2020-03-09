using System;
using System.Threading;

namespace Breaker.Core.States
{
    public class HalfOpenState : CircuitBreakerState
    {
        private readonly int _successThreshold;

        public HalfOpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker) => _successThreshold = CircuitBreaker.Settings.SuccessThreshold;

        public override void OnEnter() => CircuitBreaker.SuccessCount = 0;

        public override void InvocationSucceeds()
        {
            if (SuccessThresholdReached())
            {
                CircuitBreaker.TripTo(new ClosedState(CircuitBreaker));
            }
        }

        private bool SuccessThresholdReached() => Interlocked.Increment(ref CircuitBreaker.SuccessCount) == _successThreshold;

        public override void InvocationFails(Exception e) => CircuitBreaker.TripTo(new OpenState(CircuitBreaker));
    }
}