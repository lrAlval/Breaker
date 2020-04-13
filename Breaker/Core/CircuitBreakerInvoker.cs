using System;
using System.Threading.Tasks;
using Breaker.Utils;

namespace Breaker.Core
{
    public class CircuitBreakerInvoker : ICircuitBreakerInvoker
    {
        private readonly CircuitBreakerState _currentState;
        private readonly CustomTaskFactory _taskFactory;

        private readonly TaskScheduler _customTaskScheduler;
        private readonly TimeSpan _timeout;

        public CircuitBreakerInvoker(CircuitBreakerState state, TimeSpan serviceTimeout, TaskScheduler taskScheduler = null)
        {
            _currentState = state;
            _taskFactory = new CustomTaskFactory(serviceTimeout, taskScheduler);
        }

        public void InvokeScheduled(Action onTimeInterval, TimeSpan interval) => DisposableSelfTimer.Execute(onTimeInterval, interval);

        public void InvokeThrough(Action action)
        {
            try
            {
                _taskFactory.Run(action);

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
                var result = _taskFactory.Run(func).GetAwaiter().GetResult();

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
                await _taskFactory.Run(func);

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
                var result = await _taskFactory.Run(func);

                _currentState.InvocationSucceeds();

                return result;
            }
            catch (Exception e)
            {
                _currentState.InvocationFails(e);

                return await Task.FromException<T>(e);
            }
        }
    }
}