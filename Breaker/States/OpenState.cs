using System;
using System.Threading.Tasks;

namespace CircuitBreaker.States
{
    public class OpenState : CircuitBreakerState
    {
        public OpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker)
        {
            new CircuitBreakerInvoker(this, CircuitBreaker.Settings.TaskScheduler, TimeSpan.Zero)
                    .InvokeScheduled(() => CircuitBreaker.TripTo(new HalfOpenState(CircuitBreaker)), CircuitBreaker.Settings.ResetTimeOut);
        }

        public override void OnEnter() => CircuitBreaker.SuccessCount = 0;

        public override void Execute(Action action) => throw new CircuitBreakerOpenException();
        public override T Execute<T>(Func<T> func) => throw new CircuitBreakerOpenException();
        public override Task ExecuteAsync(Func<Task> func) => throw new CircuitBreakerOpenException();
        public override Task<T> ExecuteAsync<T>(Func<Task<T>> func) => throw new CircuitBreakerOpenException();
    }
}