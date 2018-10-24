using System;
using System.Threading.Tasks;

namespace CircuitBreaker.States
{
    public class OpenState : CircuitBreakerState
    {
        public OpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker) =>
            new CircuitBreakerInvoker()
            .InvokeScheduled(() => CircuitBreaker.TripToHalfOpenState(), CircuitBreaker.Settings.ResetTimeOut);

        public override void Invoke(Action action) => throw new CircuitBreakerOpenException();
        public override T Invoke<T>(Func<T> func) => throw new CircuitBreakerOpenException();
        public override Task InvokeAsync(Func<Task> func) => throw new CircuitBreakerOpenException();
        public override Task<T> InvokeAsync<T>(Func<Task<T>> func) => throw new CircuitBreakerOpenException();
    }
}
