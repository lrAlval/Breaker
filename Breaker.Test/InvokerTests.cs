using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Breaker.Core.Test.Fixtures;

namespace Breaker.Core.Test
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
            await Invoker.InvokeThroughAsync(() => throw new Exception());
            CurrentState.Received().InvocationFails(Arg.Any<Exception>());
        }

        [Test]
        public async Task SuccessfulFuncInvocation_ItShouldCallInvocationSuccess()
        {
            await Invoker.InvokeThroughAsync(() => Task.FromResult(false));
            CurrentState.Received().InvocationSucceeds();
        }
    }
}