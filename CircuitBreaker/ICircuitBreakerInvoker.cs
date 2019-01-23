using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    interface ICircuitBreakerInvoker
    {
        void InvokeScheduled(Action action, TimeSpan interval);
        void InvokeThrough(CircuitBreakerState state, Action action, TimeSpan timeout);
        T InvokeThrough<T>(CircuitBreakerState state, Func<T> func, TimeSpan timeout);
        Task InvokeThroughAsync(CircuitBreakerState state, Func<Task> func, TimeSpan timeout);
        Task<T> InvokeThroughAsync<T>(CircuitBreakerState state, Func<Task<T>> func, TimeSpan timeout);
    }
}