using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreakerConfig
    {
        public int SuccessThreshold { get; set; }
        public int FailuresThreshold { get; set; }
        public TimeSpan InvocationTimeOut { get; set; }
        public TimeSpan ResetTimeOut { get; set; }
        public TaskScheduler TaskScheduler { get; set; } = TaskScheduler.Default;
    }
}