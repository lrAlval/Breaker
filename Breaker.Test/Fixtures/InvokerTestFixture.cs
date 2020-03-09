using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Breaker.Core.Test.Fixtures
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
            Invoker = new CircuitBreakerInvoker(CurrentState, TimeSpan.FromMilliseconds(80));
        }
    }
}