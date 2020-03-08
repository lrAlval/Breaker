using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreakerInvoker : ICircuitBreakerInvoker
    {
        private readonly CircuitBreakerState _currentState;
        private readonly TaskScheduler _customTaskScheduler;
        private readonly TimeSpan _timeout;

        public CircuitBreakerInvoker(CircuitBreakerState state, TaskScheduler taskScheduler, TimeSpan timeout)
        {
            _currentState = state;
            _customTaskScheduler = taskScheduler;
            _timeout = timeout;
        }

        public void InvokeScheduled(Action action, TimeSpan interval)
        {
            Timer timer = null;
            timer = new Timer(_ => { action(); timer.Dispose(); }, null, (int)interval.TotalMilliseconds, Timeout.Infinite);
        }

        public void InvokeThrough(Action action)
        {
            try
            {
                Invoke(action, _timeout);

                _currentState.InvocationSucceeds();
            }
            catch (Exception e)
            {
                _currentState.InvocationFails(e);
            }
        }

        public T InvokeThrough<T>(Func<T> func)
        {
            try
            {
                var result = Invoke(func, _timeout);

                _currentState.InvocationSucceeds();

                return result;
            }
            catch (Exception e)
            {
                _currentState.InvocationFails(e);

                return default;
            }
        }

        public async Task InvokeThroughAsync(Func<Task> func)
        {
            try
            {
                await InvokeAsync(func, _timeout);

                _currentState.InvocationSucceeds();
            }
            catch (Exception e)
            {
                _currentState.InvocationFails(e);
            }

        }

        public async Task<T> InvokeThroughAsync<T>(Func<Task<T>> func)
        {
            try
            {
                var result = await InvokeAsync(func, _timeout);

                _currentState.InvocationSucceeds();

                return result;
            }
            catch (Exception e)
            {
                _currentState.InvocationFails(e);

                return await Task.FromException<T>(e);
            }
        }

        private void Invoke(Action action, TimeSpan timeout) => Schedule(action).TimeoutAfter(timeout).Wait();

        private T Invoke<T>(Func<T> func, TimeSpan timeout) => Schedule(func).TimeoutAfter(timeout).GetAwaiter().GetResult();

        private Task InvokeAsync(Func<Task> func, TimeSpan timeout) => Schedule(func).Unwrap().TimeoutAfter(timeout);

        private Task<T> InvokeAsync<T>(Func<Task<T>> func, TimeSpan timeout) => Schedule(func).Unwrap().TimeoutAfter(timeout);

        private Task<T> Schedule<T>(Func<T> func, CancellationToken cancellationToken = default)
            => Task.Factory.StartNew(func, cancellationToken, TaskCreationOptions.DenyChildAttach, _customTaskScheduler);

        private Task Schedule(Action action, CancellationToken cancellationToken = default)
            => Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.DenyChildAttach, _customTaskScheduler);
    }
}