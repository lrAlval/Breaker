using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CircuitBreaker.Test.Fixtures
{
    [TestFixture]
    public abstract class InvokerTestFixture
    {
        protected CircuitBreakerInvoker Invoker;
        protected CircuitBreakerState CurrentState;

        [SetUp]
        public void Init()
        {
            var circuitBreaker = new CircuitBreaker(new CircuitBreakerConfig());
            CurrentState = Substitute.For<CircuitBreakerState>(circuitBreaker);
            Invoker = new CircuitBreakerInvoker(CurrentState, TaskScheduler.Default, TimeSpan.FromMilliseconds(80));
        }
    }
}