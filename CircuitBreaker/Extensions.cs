using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public static class Extensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false))
                return await task.ConfigureAwait(false);

            throw new CircuitBreakerTimeoutException();
        }

        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false))
                await task.ConfigureAwait(false);

            throw new CircuitBreakerTimeoutException();
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