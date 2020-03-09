using System;
using System.Threading.Tasks;

namespace Breaker.Core
{
    public abstract class CircuitBreakerState
    {
        protected readonly CircuitBreaker CircuitBreaker;
        protected readonly CircuitBreakerInvoker _invoker;

        protected CircuitBreakerState(CircuitBreaker circuitBreaker)
        {
            CircuitBreaker = circuitBreaker;
            _invoker = new CircuitBreakerInvoker(this, CircuitBreaker.Settings.InvocationTimeOut, CircuitBreaker.Settings.TaskScheduler);
        }

        public abstract void OnEnter();

        public virtual void InvocationSucceeds() { }

        public virtual void InvocationFails(Exception e) { }

        public virtual void Execute(Action action) => _invoker.InvokeThrough(action);

        public virtual T Execute<T>(Func<T> func) => _invoker.InvokeThrough(func);

        public virtual Task ExecuteAsync(Func<Task> func) => _invoker.InvokeThroughAsync(func);

        public virtual Task<T> ExecuteAsync<T>(Func<Task<T>> func) => _invoker.InvokeThroughAsync(func);
    }
}