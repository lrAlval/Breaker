using FluentAssertions;
using NUnit.Framework;
using System;

namespace CircuitBreaker.Test
{
    [TestFixture]
    public class HalfOpenState
    {
        private CircuitBreaker _circuitBreaker;

        [SetUp]
        public void Init()
        {
            _circuitBreaker = new CircuitBreaker(new CircuitBreakerConfig
            {
                ResetTimeOut = TimeSpan.FromMilliseconds(5),
                InvocationTimeOut = TimeSpan.FromMilliseconds(7),
                FailuresThreshold = 1,
                SuccessThreshold = 1
            });

            _circuitBreaker.TripToHalfOpenState();
        }

        [Test]
        [NonParallelizable]
        public void GivenInHalfOpenState_WhenInvocationFails_ThenItShouldTripToOpen()
        {
            _circuitBreaker.Execute(() => throw new Exception());
            _circuitBreaker.IsOpen.Should().BeTrue();
        }

        [Test]
        [NonParallelizable]
        public void GivenInHalfOpenState_WhenInvocationIsSuccessful_ThenItShouldTripToClose()
        {
            _circuitBreaker.Execute(() => { });
            _circuitBreaker.IsClosed.Should().BeTrue();
        }

        [Test]
        [NonParallelizable]
        public void GivenInHalfOpenState_WhenInvocationIsSuccessful_ThenFailureCountShouldRemainInZero()
        {
            _circuitBreaker.Execute(() => { });
            _circuitBreaker.FailureCount.Should().Be(0);
        }
    }
}
