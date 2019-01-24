using FluentAssertions;
using System;
using System.Threading;
using NUnit.Framework;

namespace CircuitBreaker.Test
{
    [NonParallelizable]
    [TestFixture]
    public class OpenState
    {
        private  CircuitBreaker _circuitBreaker;

        [SetUp]
        public void Init()
        {
            _circuitBreaker = new CircuitBreaker(new CircuitBreakerConfig
            {
                ResetTimeOut = TimeSpan.FromMilliseconds(30),
                InvocationTimeOut = TimeSpan.FromMilliseconds(7),
                FailuresThreshold = 2,
                SuccessThreshold = 1
            });

            _circuitBreaker.TripToOpenState();
        }

        [Test]
        public void GivenInOpenState_WhenInvoke_ItShouldFailFast()
        {
            var countInvocation = 0;
            
            Action sut = () => _circuitBreaker.Execute(() => countInvocation++);

            sut.Should().Throw<CircuitBreakerOpenException>();
            countInvocation.Should().Be(0);
        }

        [Test]
        public void GivenInOpenState_WhenResetTimeOutPassed_ThenItShouldTripToHalfOpen()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(50));

            _circuitBreaker.IsHalfOpen.Should().BeTrue();
        }

        [Test]
        public void GivenInOpenState_WhenTripToClose_ThenItShouldBeInCloseState()
        {
            _circuitBreaker.IsOpen.Should().BeTrue();
            _circuitBreaker.TripToClosedState();
            _circuitBreaker.IsClosed.Should().BeTrue();
        }
    }
}
