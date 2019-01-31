using CircuitBreaker.States;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    //TODO Add CircuitBreakerCommands with unique identifier for every service
    public class CircuitBreaker
    {
        public event Action<CircuitBreakerState> OnStateChange;

        public CircuitBreakerConfig Settings;
        public int FailureCount;
        public int SuccessCount;

        private CircuitBreakerState _currentState;

        public bool IsClosed => _currentState is ClosedState;
        public bool IsOpen => _currentState is OpenState;
        public bool IsHalfOpen => _currentState is HalfOpenState;

        public CircuitBreaker(CircuitBreakerConfig settings)
        {
            Settings = settings;
            _currentState = new ClosedState(this);
            _currentState.OnEnter();
        }

        public void TripTo(CircuitBreakerState to)
        {
            if (TryToTrip(_currentState, to))
            {
                NotifyStateChange();
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

        public void Execute(Action action) => _currentState.Execute(action.ThrowIfNull(nameof(action)));

        public T Execute<T>(Func<T> func) => _currentState.Execute(func.ThrowIfNull(nameof(func)));

        public Task ExecuteAsync(Func<Task> func) => _currentState.ExecuteAsync(func.ThrowIfNull(nameof(func)));

        public Task<T> ExecuteAsync<T>(Func<Task<T>> func) => _currentState.ExecuteAsync(func.ThrowIfNull(nameof(func)));

        private void NotifyStateChange() => OnStateChange?.Invoke(_currentState);
    }

    public class CircuitBreakerOpenException : Exception { }
    public class CircuitBreakerTimeoutException : Exception
    {
        public CircuitBreakerTimeoutException(string message = "The operation has timed out.") : base(message) { }
    }
}