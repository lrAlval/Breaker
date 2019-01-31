using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace CircuitBreaker.Test.Fixtures
{
    [TestFixture]
    public abstract class CircuitBreakerStateFixture
    {
        protected CircuitBreaker CircuitBreaker;
        protected CircuitBreakerState CurrentState;

        [SetUp]
        public virtual void Init()
        {
            var settings = new CircuitBreakerConfig
            {
                ResetTimeOut = TimeSpan.FromMilliseconds(40),
                InvocationTimeOut = TimeSpan.FromMilliseconds(250),
                FailuresThreshold = 2,
                SuccessThreshold = 1,
                TaskScheduler = TaskScheduler.Default
            };
            CircuitBreaker = Substitute.For<CircuitBreaker>(settings);
            CurrentState = InitialState();
        }

        public abstract CircuitBreakerState InitialState();
    }
}