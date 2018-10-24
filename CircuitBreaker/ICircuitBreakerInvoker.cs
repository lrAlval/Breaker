using System;

namespace CircuitBreaker
{
    interface ICircuitBreakerInvoker
    {
        void InvokeScheduled(Action action, TimeSpan interval);
        void InvokeThrough(CircuitBreakerState state, Action action, TimeSpan timeout);
        T InvokeThrough<T>(CircuitBreakerState state, Func<T> func, TimeSpan timeout);
    }
}
