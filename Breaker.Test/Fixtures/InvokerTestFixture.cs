using NSubstitute;
using NUnit.Framework;
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
            Invoker = new CircuitBreakerInvoker(TaskScheduler.Default);
            CurrentState = Substitute.For<CircuitBreakerState>(circuitBreaker);
        }
    }

}