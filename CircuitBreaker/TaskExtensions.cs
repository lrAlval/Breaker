using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public static class TaskExtensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
                return await task;
            
            throw new CircuitBreakerTimeoutException();
        }

        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
                await task;

            throw new CircuitBreakerTimeoutException();
        }
    }
}
