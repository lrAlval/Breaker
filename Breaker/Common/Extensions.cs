using System;
using System.Threading;
using System.Threading.Tasks;
using Breaker.Core;

namespace Breaker.Core
{
    public static class Extensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task.ConfigureAwait(false);
                }

                throw new CircuitBreakerTimeoutException();
            }
        }

        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task.ConfigureAwait(false);
                }
                else
                {
                    throw new CircuitBreakerTimeoutException();
                }
            }
        }

        public static T ThrowIfNull<T>(this T argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentException(argumentName);
            }

            return argument;
        }
    }
}