using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreakerInvoker : ICircuitBreakerInvoker
    {
        private readonly TaskScheduler _customTaskScheduler;

        public CircuitBreakerInvoker(TaskScheduler taskScheduler) => _customTaskScheduler = taskScheduler;

        public void InvokeScheduled(Action action, TimeSpan interval) => Task.Delay(interval).ContinueWith(_ => action());

        public void InvokeThrough(CircuitBreakerState state, Action action, TimeSpan timeout)
        {
            try
            {
                Invoke(action, timeout);
                state.InvocationSucceeds();
            }
            catch (Exception e)
            {
                state.InvocationFails(e);
            }
        }

        public T InvokeThrough<T>(CircuitBreakerState state, Func<T> func, TimeSpan timeout)
        {
            try
            {
                var result = Invoke(func, timeout);
                state.InvocationSucceeds();
                return result;
            }
            catch (Exception e)
            {
                state.InvocationFails(e);
                return default(T);
            }
        }

        public Task InvokeThroughAsync(CircuitBreakerState state, Func<Task> func, TimeSpan timeout)
        {
            try
            {
                var result = InvokeAsync(func, timeout);
                state.InvocationSucceeds();
                return result;
            }
            catch (Exception e)
            {
                state.InvocationFails(e);
                return Task.FromException(e);
            }

        }

        public Task<T> InvokeThroughAsync<T>(CircuitBreakerState state, Func<Task<T>> func, TimeSpan timeout)
        {
            try
            {
                var result = InvokeAsync(func.ThrowIfNull(nameof(func)), timeout);
                state.InvocationSucceeds();
                return result;
            }
            catch (Exception e)
            {
                state.InvocationFails(e);
                return Task.FromException<T>(e);
            }
        }

        private void Invoke(Action action, TimeSpan timeout)
        {
            var tokenSource = new CancellationTokenSource();
            var task = Schedule(action, _customTaskScheduler, tokenSource.Token);
            if (task.IsCompleted || task.Wait((int)timeout.TotalMilliseconds, tokenSource.Token))
            {
                return;
            }

            tokenSource.Cancel();
            throw new CircuitBreakerTimeoutException();
        }

        private T Invoke<T>(Func<T> func, TimeSpan timeout)
        {
            var tokenSource = new CancellationTokenSource();
            var task = Schedule(func, _customTaskScheduler, tokenSource.Token);
            if (task.IsCompleted || task.Wait((int)timeout.TotalMilliseconds, tokenSource.Token))
            {
                return task.Result;
            }

            tokenSource.Cancel();
            throw new CircuitBreakerTimeoutException();
        }

        private Task InvokeAsync(Func<Task> func, TimeSpan timeout) => Schedule(func, _customTaskScheduler).Unwrap().TimeoutAfter(timeout);

        private Task<T> InvokeAsync<T>(Func<Task<T>> func, TimeSpan timeout) => Schedule(func, _customTaskScheduler).Unwrap().TimeoutAfter(timeout);

        private Task<T> Schedule<T>(Func<T> func, TaskScheduler scheduler, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Factory.StartNew(func, cancellationToken, TaskCreationOptions.DenyChildAttach, scheduler);

        private Task Schedule(Action action, TaskScheduler scheduler, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.DenyChildAttach, scheduler);

    }
}