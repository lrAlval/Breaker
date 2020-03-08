using System;
using CircuitBreaker.States;
using FluentAssertions;
using NSubstitute;
using CircuitBreaker.Test.Fixtures;
using NUnit.Framework;

namespace CircuitBreaker.Test
{
    public class CloseState : CircuitBreakerStateFixture
    {
        public override CircuitBreakerState InitialState() => new ClosedState(CircuitBreaker);
        
        [Test]
        public void WhenFailureThresholdIsReached_WhenFailureThresholdIsReached()
        {
            CurrentState.InvocationFails(Arg.Any<Exception>());
            CircuitBreaker.IsOpen.Should().BeFalse();
            CurrentState.InvocationFails(Arg.Any<Exception>());
            CircuitBreaker.IsOpen.Should().BeTrue();
        }
    }
}