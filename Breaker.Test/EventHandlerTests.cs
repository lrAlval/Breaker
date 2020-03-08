using CircuitBreaker.Test.Fixtures;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CircuitBreaker.Test
{
    public class EventNotifierTests : EventNotifierFixture
    {
        [Test]
        public async Task WhenStateChanges_ItShouldNotifyTheRightState()
        {
            var raisedStates = new List<CircuitBreakerState>();

            CircuitBreaker.OnStateChange += newState => raisedStates.Add(newState);

            //Trigger two Failures to Trip to Open
            CircuitBreaker.Execute(() => throw new Exception());
            CircuitBreaker.Execute(() => throw new Exception());

            //Wait for the Timeout in order to Trip HalfOpen
            await Task.Delay(CircuitBreaker.Settings.ResetTimeOut);
            await Task.Delay(1);

            //Trigger one success invoke in HalfOpen State to Trip to Close
            CircuitBreaker.Execute(() => { });

            raisedStates.Should().NotBeEmpty()
                .And.HaveCount(3)
                .And.ContainItemsAssignableTo<CircuitBreakerState>();
        }
    }
}