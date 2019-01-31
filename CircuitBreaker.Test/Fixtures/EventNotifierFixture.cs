using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CircuitBreaker.Test.Fixtures
{
    [TestFixture]
    public abstract class EventNotifierFixture
    {
        protected CircuitBreaker CircuitBreaker;

        [SetUp]
        public void Init()
        {
            CircuitBreaker = new CircuitBreaker(new CircuitBreakerConfig
            {
                ResetTimeOut = TimeSpan.FromMilliseconds(40),
                InvocationTimeOut = TimeSpan.FromMilliseconds(500),
                TaskScheduler = TaskScheduler.Default,
                FailuresThreshold = 2,
                SuccessThreshold = 1,
            });
        }
    }
}