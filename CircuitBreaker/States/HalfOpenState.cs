using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker.States
{
    public class HalfOpenState : CircuitBreakerState
    {
        private readonly TimeSpan _timeout;
        private readonly object _lockObject = new object();
        private readonly ICircuitBreakerInvoker _invoker = new CircuitBreakerInvoker();

        public HalfOpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker)
        {
            CircuitBreaker.SuccessCount = 0;
            _timeout = this.CircuitBreaker.Settings.InvocationTimeOut;
        }

        public override CircuitBreakerState InvocationSucceeds()
        {
            Interlocked.Increment(ref CircuitBreaker.SuccessCount);

            return SuccessThresholdReached() ? CircuitBreaker.TripToClosedState() : this;
        }

        public override CircuitBreakerState InvocationFails(Exception e) => CircuitBreaker.TripToOpenState();

        private bool SuccessThresholdReached() => CircuitBreaker.SuccessCount == CircuitBreaker.Settings.SuccessThreshold;

        public override void Invoke(Action action) => _invoker.InvokeThrough(this, action, _timeout);

        public override T Invoke<T>(Func<T> func) => _invoker.InvokeThrough(this, func, _timeout);
        
        public override Task InvokeAsync(Func<Task> func) => _invoker.InvokeThrough(this, func, _timeout);

        public override Task<T> InvokeAsync<T>(Func<Task<T>> func) => _invoker.InvokeThrough(this, func, _timeout);

    }
}
