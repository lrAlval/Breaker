using System;
using System.Threading;
using System.Threading.Tasks;
using Breaker.Common;

namespace Breaker.Utils
{
    public class CustomTaskFactory
    {
        private readonly TaskFactory _taskFactory;
        private readonly TimeSpan _serviceTimeout;

        public CustomTaskFactory(TimeSpan serviceTimeout, TaskScheduler taskScheduler = null)
        {
            _serviceTimeout = serviceTimeout;
            _taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.None, taskScheduler ?? TaskScheduler.Default);
        }

        public Task Run(Action func, CancellationTokenSource tokenSource = default)
        {
            using (tokenSource ??= new CancellationTokenSource())
            {
                return _taskFactory.StartNew(func, tokenSource.Token).TimeoutAfter(_serviceTimeout, tokenSource);
            }
        }

        public Task<T> Run<T>(Func<T> func, CancellationTokenSource tokenSource = default)
        {
            using (tokenSource ??= new CancellationTokenSource())
            {
                return _taskFactory.StartNew(func, tokenSource.Token).TimeoutAfter(_serviceTimeout, tokenSource);
            }
        }

        public Task Run(Func<Task> func, CancellationTokenSource tokenSource = default)
        {
            using (tokenSource ??= new CancellationTokenSource())
            {
                return _taskFactory.StartNew(func, tokenSource.Token).Unwrap().TimeoutAfter(_serviceTimeout, tokenSource);
            }
        }

        public Task<T> Run<T>(Func<Task<T>> func, CancellationTokenSource tokenSource = default)
        {
            using (tokenSource ??= new CancellationTokenSource())
            {
                return _taskFactory.StartNew(func, tokenSource.Token).Unwrap().TimeoutAfter(_serviceTimeout, tokenSource);
            }
        }
    }
}