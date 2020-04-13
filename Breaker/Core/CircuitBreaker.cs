using System;
using System.Threading;
using System.Threading.Tasks;
using Breaker.Core.States;
using Breaker.Events;

namespace Breaker.Core
{
    //TODO Add CircuitBreakerCommands with unique identifier for every service
    public class CircuitBreaker
    {
        private CircuitBreakerState _currentState;

        public CircuitBreaker(CircuitBreakerConfig settings)
        {
            Settings = settings;

            Notifier = new CircuitBreakerEvents();

            _currentState = new ClosedState(this);
            _currentState.OnEnter();
        }

        public int FailureCount;
        public int SuccessCount;

        
        public CircuitBreakerEvents Notifier { get; set; }
        public CircuitBreakerConfig Settings { get; set; }

        public bool IsClosed => _currentState is ClosedState;
        public bool IsOpen => _currentState is OpenState;
        public bool IsHalfOpen => _currentState is HalfOpenState;

        public void TripTo(CircuitBreakerState to)
        {
            if (TryToTrip(_currentState, to))
            {
                Notifier.NotifyStateChange(_currentState);
            }
        }

        private bool TryToTrip(CircuitBreakerState from, CircuitBreakerState to)
        {
            if (Interlocked.CompareExchange(ref _currentState, to, from) == from)
            {
                to.OnEnter();

                return true;
            }

            return false;
        }

        public void Execute(Action action) => _currentState.Execute(action);

        public T Execute<T>(Func<T> func) => _currentState.Execute(func);

        public Task ExecuteAsync(Func<Task> func) => _currentState.ExecuteAsync(func);

        public Task<T> ExecuteAsync<T>(Func<Task<T>> func) => _currentState.ExecuteAsync(func);
    }
}