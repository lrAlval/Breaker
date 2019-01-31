using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using CircuitBreaker.Test.Fixtures;

namespace CircuitBreaker.Test
{
    public class InvokeScheduledTests : InvokerTestFixture
    {
        [Test]
        public void ScheduledAction_AfterSomeTimeItShouldInvokeTheAction()
        {
            var isExecuted = false;
            var reset = new ManualResetEventSlim(false);

            Invoker.InvokeScheduled(() => { isExecuted = true; reset.Set(); }, TimeSpan.FromMilliseconds(80));

            reset.Wait();
            isExecuted.Should().BeTrue();
        }
    }

    public class InvokeThroughAsyncTests : InvokerTestFixture
    {
        [Test]
        public async Task FailedFuncInvocation_ItShouldCallInvocationFails()
        {
            await Invoker.InvokeThroughAsync(CurrentState, () => throw new Exception(), TimeSpan.FromMilliseconds(100));
            CurrentState.Received().InvocationFails(Arg.Any<Exception>());
        }

        [Test]
        public async Task SuccessfulFuncInvocation_ItShouldCallInvocationFails()
        {
            await Invoker.InvokeThroughAsync(CurrentState, () => Task.FromResult(false), TimeSpan.FromMilliseconds(100));
            CurrentState.Received().InvocationSucceeds();
        }
    }
}