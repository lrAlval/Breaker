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
            _invoker = new CircuitBreakerInvoker(CircuitBreaker.Settings.TaskScheduler);
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

            return IsThresholdReached ? CircuitBreaker.TripToOpenState() : this;
        }

        public override void Execute(Action action) => _invoker.InvokeThrough(this, action, _timeout);

        public override T Execute<T>(Func<T> func) => _invoker.InvokeThrough(this, func, _timeout);

        public override Task ExecuteAsync(Func<Task> func) => _invoker.InvokeThroughAsync(this, func, _timeout);

        public override Task<T> ExecuteAsync<T>(Func<Task<T>> func) => _invoker.InvokeThroughAsync(this, func, _timeout);

        private bool IsThresholdReached => CircuitBreaker.FailureCount == _maxFailures;
    }
}