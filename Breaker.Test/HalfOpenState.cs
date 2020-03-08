using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using CircuitBreaker.Test.Fixtures;

namespace CircuitBreaker.Test
{
    public class HalfOpenState : CircuitBreakerStateFixture
    {
        public override CircuitBreakerState InitialState() => new States.HalfOpenState(CircuitBreaker);

        [Test]
        public void WhenInvocationFails_ThenItShouldTripToOpen()
        {
            CurrentState.InvocationFails(Arg.Any<Exception>());
            CurrentState.InvocationFails(Arg.Any<Exception>());
            CircuitBreaker.IsOpen.Should().BeTrue();
        }

        [Test]
        public void WhenInvocationIsSuccessful_ThenItShouldTripToClose()
        {
            CurrentState.InvocationSucceeds();
            CircuitBreaker.IsClosed.Should().BeTrue();
        }
    }
}