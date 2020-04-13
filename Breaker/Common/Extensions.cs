using System;
using System.Threading;
using System.Threading.Tasks;
using Breaker.Core;

namespace Breaker.Common
{
    public static class Extensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> primaryTask, TimeSpan timeout, CancellationTokenSource primaryTaskTokenSource = default)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(primaryTask, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
                if (completedTask == primaryTask)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await primaryTask.ConfigureAwait(false);
                }

                if (primaryTaskTokenSource != null && !primaryTaskTokenSource.IsCancellationRequested)
                {
                    primaryTaskTokenSource.Cancel();
                    primaryTaskTokenSource.Dispose();
                }

                throw new CircuitBreakerTimeoutException();
            }
        }

        public static async Task TimeoutAfter(this Task primaryTask, TimeSpan timeout, CancellationTokenSource primaryTaskTokenSource = default)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(primaryTask, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
                if (completedTask == primaryTask)
                {
                    timeoutCancellationTokenSource.Cancel();
                    await primaryTask.ConfigureAwait(false);
                }
                else
                {
                    if (primaryTaskTokenSource != null && !primaryTaskTokenSource.IsCancellationRequested)
                    {
                        primaryTaskTokenSource.Cancel();
                        primaryTaskTokenSource.Dispose();
                    }

                    throw new CircuitBreakerTimeoutException();
                }
            }
        }
    }
}