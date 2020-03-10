using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Breaker.Core;
using Breaker.Core.States;

namespace Breaker.Benchmarks
{

    [SimpleJob(launchCount: 1, warmupCount: 3, targetCount: 5, invocationCount: 100, id: "QuickJob")]
    [ThreadingDiagnoser]
    [MemoryDiagnoser]
    [BenchmarkCategory("InvokerBench")]
    public class InvokerBench
    {
        private CircuitBreaker _circuitBreaker;
        private CircuitBreakerInvoker _invoker;

        [Params(100, 1000)]
        public int N { get; set; }


        [GlobalSetup]
        public void SetUp()
        {
            var invocationTimeout = TimeSpan.FromMilliseconds(500);

            _circuitBreaker = new CircuitBreaker(new CircuitBreakerConfig
            {
                ResetTimeOut = TimeSpan.FromMilliseconds(40),
                InvocationTimeOut = TimeSpan.FromMilliseconds(500),
                TaskScheduler = TaskScheduler.Default,
                FailuresThreshold = 2,
                SuccessThreshold = 1,
            });

            var closedState = new ClosedState(_circuitBreaker);

            _invoker = new CircuitBreakerInvoker(closedState, invocationTimeout);
        }

        [Benchmark]
        public async Task Run()
        {
            for (int i = 0; i < N; i++)
            {
                await _invoker.InvokeThroughAsync(() => Task.FromResult(false));
            }
        }
    }
}