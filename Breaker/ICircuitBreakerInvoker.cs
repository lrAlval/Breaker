using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    interface ICircuitBreakerInvoker
    {
        void InvokeScheduled(Action action, TimeSpan interval);
        void InvokeThrough(Action action);
        T InvokeThrough<T>(Func<T> func);
        Task InvokeThroughAsync(Func<Task> func);
        Task<T> InvokeThroughAsync<T>(Func<Task<T>> func);
    }
}