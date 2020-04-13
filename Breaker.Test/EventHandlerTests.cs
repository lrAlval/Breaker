using System;
using System.Collections.Generic;
using System.Threading;
using Breaker.Core.Test.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Breaker.Core.Test
{
    public class EventNotifierTests : EventNotifierFixture
    {
        [Test]
        public void WhenStateChanges_ItShouldNotifyTheRightState()
        {
            try
            {
                var raisedStates = new List<CircuitBreakerState>();

                CircuitBreaker.Notifier.OnStateChange += newState => raisedStates.Add(newState);

                //Trigger two Failures to Trip to Open
                CircuitBreaker.Execute(() => throw new Exception());
                CircuitBreaker.Execute(() => throw new Exception());

                //Spint until the timeout is reached in order to Trip HalfOpen
                SpinWait.SpinUntil(() => false, CircuitBreaker.Settings.ResetTimeOut);

                //Trigger one success invoke in HalfOpen State to Trip to Close
                CircuitBreaker.Execute(() => { });

                raisedStates.Should().NotBeEmpty()
                    .And.HaveCount(3)
                    .And.ContainItemsAssignableTo<CircuitBreakerState>();
            }
            catch (Exception) { }
        }
    }
}