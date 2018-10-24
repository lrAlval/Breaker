using FluentAssertions;
using System;
using Xunit;

namespace CircuitBreaker.Test
{
    public class HalfOpenState
    {
        private readonly CircuitBreaker _circuitBreaker;

        public HalfOpenState()
        {
            _circuitBreaker = new CircuitBreaker(new CircuitBreakerConfig
            {
                ResetTimeOut = TimeSpan.FromMilliseconds(5),
                InvocationTimeOut = TimeSpan.FromMilliseconds(7),
                FailuresThreshold = 2,
                SuccessThreshold = 1
            });

            _circuitBreaker.TripToHalfOpenState();
        }

        [Fact]
        public void GivenInHalfOpenState_WhenInvocationFails_ThenItShouldTripToOpen()
        {
            _circuitBreaker.Execute(() => throw new Exception());
            _circuitBreaker.IsOpen.Should().BeTrue();
        }

        [Fact]
        public void GivenInHalfOpenState_WhenInvocationIsSuccessful_ThenItShouldTripToClose()
        {
            _circuitBreaker.Execute(() => { });
            _circuitBreaker.IsClosed.Should().BeTrue();
        }

        [Fact]
        public void GivenInHalfOpenState_WhenInvocationIsSuccessful_ThenFailureCountShouldRemainInZero()
        {
            _circuitBreaker.Execute(() => { });
            _circuitBreaker.FailureCount.Should().Be(0);
        }
    }
}
