using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker.States
{
    public class ClosedState : CircuitBreakerState
    {
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly int _maxFailures;
        private readonly TimeSpan _timeout;

        public ClosedState(CircuitBreaker circuitBreaker) : base(circuitBreaker)
        {
            _invoker = new CircuitBreakerInvoker();
            _maxFailures = CircuitBreaker.Settings.FailuresThreshold;
            _timeout = CircuitBreaker.Settings.InvocationTimeOut;
            CircuitBreaker.FailureCount = 0;
        }

        public override CircuitBreakerState InvocationSucceeds()
        {
            CircuitBreaker.FailureCount = 0;
            return this;
        }

        public override CircuitBreakerState InvocationFails(Exception e)
        {
            Interlocked.Increment(ref CircuitBreaker.FailureCount);

            return IsThresholdReached() ? CircuitBreaker.TripToOpenState() : this;
        }

        private bool IsThresholdReached() => CircuitBreaker.FailureCount == _maxFailures;

        public override void Invoke(Action action) => _invoker.InvokeThrough(this, action, _timeout);

        public override T Invoke<T>(Func<T> func) => _invoker.InvokeThrough(this, func, _timeout);

        public override Task InvokeAsync(Func<Task> func) => _invoker.InvokeThrough(this, func, _timeout);

        public override Task<T> InvokeAsync<T>(Func<Task<T>> func) => _invoker.InvokeThrough(this, func, _timeout);
    }
}
