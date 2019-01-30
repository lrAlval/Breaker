using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker.Test
{
    [TestFixture]
    public class CloseState
    {
        private CircuitBreaker _circuitBreaker;

        [SetUp]
        public void Init() => _circuitBreaker = new CircuitBreaker(new CircuitBreakerConfig
        {
            ResetTimeOut = TimeSpan.FromMilliseconds(5),
            InvocationTimeOut = TimeSpan.FromMilliseconds(7),
            FailuresThreshold = 2,
            SuccessThreshold = 1,
            TaskScheduler = TaskScheduler.Default
        });

        [Test]        
        public void GivenInCloseState_WhenInvocationHappendOnCloseState_ThenItShouldCallTheMethod()
        {
            var volatileCodeWasCalled = false;
            _circuitBreaker.Execute(() => volatileCodeWasCalled = true);
            volatileCodeWasCalled.Should().BeTrue();
        }

        [Test]
        
        public void GivenInCloseState_WhenInvocationSucceeds_ThenShouldRemainInCloseState()
        {
            _circuitBreaker.Execute(() => { });
            _circuitBreaker.IsClosed.Should().BeTrue();
        }

        [Test]
        public async Task GivenInCloseState_WhenInvocationSucceedsWithReturnValue_ThenShouldRemainInCloseStateAsync()
        {
            var volatileCode = new { IsCalled = true, Name = "Test" };
            var response = await _circuitBreaker.ExecuteAsync(() => Task.FromResult(volatileCode));
            _circuitBreaker.IsClosed.Should().BeTrue();
            response.IsCalled.Should().BeTrue();
        }

        [Test]
        public void GivenInCloseState_WhenFailureThresholdIsReached_ThenShouldTripToOpenState()
        {
            var failuresThreshold = _circuitBreaker.Settings.FailuresThreshold;

            Action sut = () => CallXAmountOfTimes(() => _circuitBreaker.Execute(() => throw new Exception()), failuresThreshold);

            sut.Should().Throw<CircuitBreakerOpenException>();
            _circuitBreaker.IsOpen.Should().BeTrue();
        }

        [Test]
        public void GivenInCloseState_WhenTripToOpen_ThenShouldNotifyWithOpenState()
        {
            _circuitBreaker.TripTo(new States.OpenState(_circuitBreaker));
            _circuitBreaker.OnStateChange += state => (state is OpenState).Should().BeTrue();
        }

        [Test]
        public void GivenInCloseState_WhenTripToOpen_ThenClose_ThenHalfClose_ItShouldNotifyThreeTimes()
        {
            var changeStateCount = 0;
            _circuitBreaker.IsClosed.Should().BeTrue();
            _circuitBreaker.OnStateChange += (state) => changeStateCount++;

            //Trigger two Failures to Trip to Open
            _circuitBreaker.Execute(() => throw new Exception());
            _circuitBreaker.Execute(() => throw new Exception());

            _circuitBreaker.IsOpen.Should().BeTrue();

            //Wait for the Timeout in order to Trip HalfOpen
            Thread.Sleep(TimeSpan.FromMilliseconds(25));

            _circuitBreaker.IsHalfOpen.Should().BeTrue();

            //Trigger one success invoke in HalfOpen State to Trip to Close
            _circuitBreaker.Execute(() => { });

            _circuitBreaker.IsClosed.Should().BeTrue();

            changeStateCount.Should().Be(3);
        }

        [Test]
        public void GivenInCloseState_WhenTripToOpen_ThenItShouldNotifyOnce()
        {
            var changeStateCount = 0;
            _circuitBreaker.OnStateChange += (state) => changeStateCount++;

            //Trigger two Failures to Trip to Open
            _circuitBreaker.Execute(() => throw new Exception());
            _circuitBreaker.Execute(() => throw new Exception());

            changeStateCount.Should().Be(1);
        }

        [Test]
        public void GivenInCloseState_WhenInvocationFails_ThenFailureCounterShouldIncrease()
        {
            _circuitBreaker.Execute(() => throw new Exception());

            _circuitBreaker.FailureCount.Should().Be(1);
        }

        [Test]
        public void GivenInCloseState_WhenInvocationSucceeds_ThenFailureCounterShouldRemainInZero()
        {
            _circuitBreaker.Execute(() => { });
            _circuitBreaker.FailureCount.Should().Be(0);
        }

        private void CallXAmountOfTimes(Action volatileCode, int limit)
        {
            for (var i = 0; i <= limit; i++)
            {
                volatileCode();
            }
        }
    }
}
