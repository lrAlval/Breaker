using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker.States
{
    public class HalfOpenState : CircuitBreakerState
    {
        private readonly TimeSpan _timeout;
        private readonly int _successThreshold;
        private readonly ICircuitBreakerInvoker _invoker;

        public HalfOpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker)
        {
            _timeout = CircuitBreaker.Settings.InvocationTimeOut;
            _successThreshold = CircuitBreaker.Settings.SuccessThreshold;
            _invoker = new CircuitBreakerInvoker(CircuitBreaker.Settings.TaskScheduler);
        }

        public override void OnEnter() => CircuitBreaker.SuccessCount = 0;

        public override void InvocationSucceeds()
        {
            if (IsSuccessThresholdReached)
            {
                CircuitBreaker.TripTo(new ClosedState(CircuitBreaker));
            }
        }

        public override void InvocationFails(Exception e) => CircuitBreaker.TripTo(new OpenState(CircuitBreaker));

        public override void Execute(Action action) => _invoker.InvokeThrough(this, action, _timeout);

        public override T Execute<T>(Func<T> func) => _invoker.InvokeThrough(this, func, _timeout);

        public override Task ExecuteAsync(Func<Task> func) => _invoker.InvokeThroughAsync(this, func, _timeout);

        public override Task<T> ExecuteAsync<T>(Func<Task<T>> func) => _invoker.InvokeThroughAsync(this, func, _timeout);

        private bool IsSuccessThresholdReached => Interlocked.Increment(ref CircuitBreaker.SuccessCount) == _successThreshold;
    }
}