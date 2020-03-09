using System;
using System.Threading;

namespace Breaker.Utils
{
    public class DisposableSelfTimer
    {
        public static void Execute(Action afterTimeInterval, TimeSpan dueTimeSpan)
        {
            Timer timer = null;
            timer = new Timer(_ =>
            {
                afterTimeInterval();
                timer.Dispose();
            }, null, dueTime: (int)dueTimeSpan.TotalMilliseconds, Timeout.Infinite);
        }
    }
}