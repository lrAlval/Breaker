using System;
using FluentAssertions;
using System.Threading.Tasks;
using CircuitBreaker.Test.Fixtures;
using NUnit.Framework;

namespace CircuitBreaker.Test
{
    public class OpenState : CircuitBreakerStateFixture
    {
        public override CircuitBreakerState InitialState() => new States.OpenState(CircuitBreaker);
        
        [Test]
        public void WhenInvoke_ItShouldFailFast()
        {
            var countInvocation = 0;
            
            Action sut = () => CurrentState.Execute(() => countInvocation++);
            
            sut.Should().Throw<CircuitBreakerOpenException>();
            countInvocation.Should().Be(0);
        }

        [Test]
        public async Task WhenResetTimeOutPassed_ThenItShouldTripToHalfOpen()
        {
            await Task.Delay(CircuitBreaker.Settings.ResetTimeOut);
            await Task.Delay(1);

            CircuitBreaker.IsHalfOpen.Should().BeTrue();
        }
    }
}