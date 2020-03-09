using System;
using System.Threading;
using System.Threading.Tasks;
using Breaker.Utils;

namespace Breaker.Core
{
    public class CircuitBreakerInvoker : ICircuitBreakerInvoker
    {
        private readonly CircuitBreakerState _currentState;
        private readonly TaskScheduler _customTaskScheduler;
        private readonly TimeSpan _timeout;

        public CircuitBreakerInvoker(CircuitBreakerState state, TimeSpan timeout, TaskScheduler taskScheduler = null)
        {
            _currentState = state;
            _customTaskScheduler = taskScheduler ?? TaskScheduler.Default;
            _timeout = timeout;
        }

        public void InvokeScheduled(Action onTimeInterval, TimeSpan interval) => DisposableSelfTimer.Execute(onTimeInterval, interval);

        public void InvokeThrough(Action action)
        {
            try
            {
                Invoke(action);

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
                var result = Invoke(func);

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
                await InvokeAsync(func);

                _currentState.InvocationSucceeds();
            }
            catch (OperationCanceledException e)
            {
                _currentState.InvocationFails(e);

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
                var result = await InvokeAsync(func);

                _currentState.InvocationSucceeds();

                return result;
            }
            catch (Exception e)
            {
                _currentState.InvocationFails(e);

                return await Task.FromException<T>(e);
            }
        }

        private void Invoke(Action action) => Schedule(action).Wait();

        private T Invoke<T>(Func<T> func) => Schedule(func).GetAwaiter().GetResult();

        private Task InvokeAsync(Func<Task> func) => Schedule(func).Unwrap();

        private Task<T> InvokeAsync<T>(Func<Task<T>> func) => Schedule(func).Unwrap();

        private Task<T> Schedule<T>(Func<T> func)
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(_timeout);

                return Task.Factory.StartNew(func, cts.Token, TaskCreationOptions.DenyChildAttach, _customTaskScheduler);
            }
        }

        private Task Schedule(Action action)
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(_timeout);

                return Task.Factory.StartNew(action, cts.Token, TaskCreationOptions.DenyChildAttach, _customTaskScheduler);
            }
        }
    }
}