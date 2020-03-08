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
            _maxFailures = CircuitBreaker.Settings.FailuresThreshold;
            _timeout = CircuitBreaker.Settings.InvocationTimeOut;
            _invoker = new CircuitBreakerInvoker(this, CircuitBreaker.Settings.TaskScheduler, _timeout);
        }

        public override void OnEnter() => CircuitBreaker.FailureCount = 0;

        public override void InvocationSucceeds() => CircuitBreaker.FailureCount = 0;

        public override void InvocationFails(Exception e)
        {
            if (FailureThresholdReached())
            {
                CircuitBreaker.TripTo(new OpenState(CircuitBreaker));
            }
        }

        public override void Execute(Action action) => _invoker.InvokeThrough(action);

        public override T Execute<T>(Func<T> func) => _invoker.InvokeThrough(func);

        public override Task ExecuteAsync(Func<Task> func) => _invoker.InvokeThroughAsync(func);

        public override Task<T> ExecuteAsync<T>(Func<Task<T>> func) => _invoker.InvokeThroughAsync(func);

        private bool FailureThresholdReached() => Interlocked.Increment(ref CircuitBreaker.FailureCount) == _maxFailures;
    }
}