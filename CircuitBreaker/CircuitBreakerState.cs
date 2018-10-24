using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public abstract class CircuitBreakerState
    {
        protected readonly CircuitBreaker CircuitBreaker;

        protected CircuitBreakerState(CircuitBreaker circuitBreaker) => CircuitBreaker = circuitBreaker;

        public void Execute(Action action) => this.Invoke(action);

        public T Execute<T>(Func<T> action) => this.Invoke(action);
        
        public virtual CircuitBreakerState InvocationSucceeds() => this;

        public virtual CircuitBreakerState InvocationFails(Exception e) => this;

        public abstract void Invoke(Action action);
        public abstract T Invoke<T>(Func<T> func);

        public abstract Task InvokeAsync(Func<Task> func);
        public abstract Task<T> InvokeAsync<T>(Func<Task<T>> func);
    }
}
