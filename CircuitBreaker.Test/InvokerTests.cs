using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker.Test
{
    [TestFixture]
    public abstract class InvokerTestBase
    {
        protected CircuitBreakerInvoker Invoker;

        [SetUp]
        public virtual void Init() => Invoker = new CircuitBreakerInvoker(TaskScheduler.Default);
    }


    public class InvokeScheduledTests : InvokerTestBase
    {
        [Test]
        public void InvokeScheduled_ScheduledAction_AfterSomeTimeItShouldInvokeTheAction()
        {
            var isExecuted = false;
            var reset = new ManualResetEventSlim(false);

            Invoker.InvokeScheduled(() => { isExecuted = true; reset.Set(); }, TimeSpan.FromMilliseconds(80));

            reset.Wait(100);
            isExecuted.Should().BeTrue();
        }
    }

    public class InvokeThroughTests : InvokerTestBase
    {
        private CircuitBreakerState _state;

        public override void Init()
        {
            base.Init();
            var settings = Substitute.For<CircuitBreakerConfig>();
            var circuitBreaker = Substitute.For<CircuitBreaker>(settings);
            _state = Substitute.For<CircuitBreakerState>(circuitBreaker);
        }

        [Test]
        public void InvokeThrough_FailedActionInvocation_ItShouldCallInvocationFails()
        {
            Invoker.InvokeThrough(_state, () => throw new Exception(), TimeSpan.FromMilliseconds(100));
            _state.Received().InvocationFails(Arg.Any<Exception>());
        }

        [Test]
        public void InvokeThrough_SuccessfulActionInvocation_ItShouldCallInvocationFails()
        {
            Invoker.InvokeThrough(_state, () => { }, TimeSpan.FromMilliseconds(100));
            _state.Received().InvocationSucceeds();
        }

        [Test]
        public void InvokeThrough_SuccessfulFuncInvocation_ItShouldCallInvocationFails()
        {
            Invoker.InvokeThrough(_state, () => new object(), TimeSpan.FromMilliseconds(100));
            _state.Received().InvocationSucceeds();
        }

        [Test]
        public void InvokeThrough_FailedFuncInvocation_ItShouldCallInvocationFails()
        {
            object Func() => throw new Exception();
            Invoker.InvokeThrough(_state, Func, TimeSpan.FromMilliseconds(100));
            _state.Received().InvocationFails(Arg.Any<Exception>());
        }
    }

    public class InvokeThroughAsyncTests : InvokerTestBase
    {
        private CircuitBreakerState _state;

        public override void Init()
        {
            base.Init();
            var settings = Substitute.For<CircuitBreakerConfig>();
            var circuitBreaker = Substitute.For<CircuitBreaker>(settings);
            _state = Substitute.For<CircuitBreakerState>(circuitBreaker);
        }

        [Test]
        public async Task InvokeThrough_FailedFuncInvocation_ItShouldCallInvocationFails()
        {
            await Invoker.InvokeThroughAsync(_state, () => throw new Exception(), TimeSpan.FromMilliseconds(100));
            _state.Received().InvocationFails(Arg.Any<Exception>());
        }

        [Test]
        public async Task InvokeThroughAsync_SuccessfulFuncInvocation_ItShouldCallInvocationFails()
        {
            await Invoker.InvokeThroughAsync(_state, () => Task.FromResult(false), TimeSpan.FromMilliseconds(100));
            _state.Received().InvocationSucceeds();
        }
    }
}