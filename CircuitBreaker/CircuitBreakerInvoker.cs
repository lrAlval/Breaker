using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreakerInvoker : ICircuitBreakerInvoker
    {
        public void InvokeScheduled(Action action, TimeSpan interval)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            Task.Delay(interval).ContinueWith(_ => action());
        }

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

        public async Task InvokeThroughAsync(CircuitBreakerState state, Func<Task> func, TimeSpan timeout)
        {
            try
            {
                await InvokeAsync(func, timeout);
            }
            catch (Exception e)
            {
                state.InvocationFails(e);
            }

            state.InvocationSucceeds();
        }

        public async Task<T> InvokeThroughAsync<T>(CircuitBreakerState state, Func<Task<T>> func, TimeSpan timeout)
        {
            Task<T> task;
            try
            {
                task = InvokeAsync(func, timeout);
                await task;
            }
            catch (Exception e)
            {
                state.InvocationFails(e);
                task = Task.FromCanceled<T>(new CancellationToken(true));
            }

            state.InvocationSucceeds();

            return await task;
        }


        private void Invoke(Action action, TimeSpan timeout)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var tokenSource = new CancellationTokenSource();
            var task = Task.Run(action, tokenSource.Token);
            if (task.IsCompleted || task.Wait((int)timeout.TotalMilliseconds, tokenSource.Token))
            {
                return;
            }

            tokenSource.Cancel();
            throw new CircuitBreakerTimeoutException();
        }

        private T Invoke<T>(Func<T> func, TimeSpan timeout)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var tokenSource = new CancellationTokenSource();
            var task = Task.Run(func, tokenSource.Token);
            if (task.IsCompleted || task.Wait((int)timeout.TotalMilliseconds, tokenSource.Token))
            {
                return task.Result;
            }

            tokenSource.Cancel();
            throw new CircuitBreakerTimeoutException();
        }

        private Task InvokeAsync(Func<Task> func, TimeSpan timeout)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return Task.Run(func).TimeoutAfter(timeout);
        }

        private Task<T> InvokeAsync<T>(Func<Task<T>> func, TimeSpan timeout)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return Task.Run(func).TimeoutAfter(timeout);
        }

        private Task<T> InvokeAsync<T>(Func<T> func, TimeSpan timeout)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return Task.Run(func).TimeoutAfter(timeout);
        }
    }
}
