using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker.States
{
    public class HalfOpenState : CircuitBreakerState
    {
        private readonly TimeSpan _timeout;
        private readonly ICircuitBreakerInvoker _invoker;

        public HalfOpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker)
        {
            CircuitBreaker.SuccessCount = 0;
            _timeout = CircuitBreaker.Settings.InvocationTimeOut;
            _invoker = new CircuitBreakerInvoker();
        }

        public override CircuitBreakerState InvocationSucceeds()
        {
            Interlocked.Increment(ref CircuitBreaker.SuccessCount);

            return IsSuccessThresholdReached ? CircuitBreaker.TripToClosedState() : this;
        }

        public override CircuitBreakerState InvocationFails(Exception e) => CircuitBreaker.TripToOpenState();
        
        public override void Execute(Action action) => _invoker.InvokeThrough(this, action, _timeout);

        public override T Execute<T>(Func<T> func) => _invoker.InvokeThrough(this, func, _timeout);
        
        public override Task ExecuteAsync(Func<Task> func) => _invoker.InvokeThroughAsync(this, func, _timeout);

        public override Task<T> ExecuteAsync<T>(Func<Task<T>> func) => _invoker.InvokeThroughAsync(this, func, _timeout);

        private bool IsSuccessThresholdReached => CircuitBreaker.SuccessCount == CircuitBreaker.Settings.SuccessThreshold;
    }
}