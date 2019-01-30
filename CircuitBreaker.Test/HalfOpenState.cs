﻿using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

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
                ResetTimeOut = TimeSpan.FromMilliseconds(15),
                InvocationTimeOut = TimeSpan.FromMilliseconds(18),
                FailuresThreshold = 1,
                SuccessThreshold = 1,
                TaskScheduler = TaskScheduler.Default
            });

            _circuitBreaker.TripTo(new States.HalfOpenState(_circuitBreaker));
        }

        [Test]
        public void GivenInHalfOpenState_WhenInvocationFails_ThenItShouldTripToOpen()
        {
            _circuitBreaker.Execute(() => throw new Exception());
            _circuitBreaker.IsOpen.Should().BeTrue();
        }

        [Test]
        public void GivenInHalfOpenState_WhenInvocationIsSuccessful_ThenItShouldTripToClose()
        {
            var volatileCodeWasCalled = false;
            _circuitBreaker.Execute(() => { volatileCodeWasCalled = true; });
            _circuitBreaker.IsClosed.Should().BeTrue();
            volatileCodeWasCalled.Should().BeTrue();
        }

        [Test]
        public void GivenInHalfOpenState_WhenInvocationIsSuccessful_ThenFailureCountShouldRemainInZero()
        {
            _circuitBreaker.Execute(() => { });
            _circuitBreaker.FailureCount.Should().Be(0);
        }
    }
}
