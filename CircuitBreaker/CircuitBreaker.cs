using CircuitBreaker.States;
using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    //TODO Add CircuitBreakerCommands with unique identifier for every service
    public class CircuitBreaker
    {
        public CircuitBreakerConfig Settings { get; set; }

        public event Action<CircuitBreakerState> OnStateChange;

        private CircuitBreakerState _state;
        
        public int FailureCount;
        public int SuccessCount;

        public bool IsClosed => _state is ClosedState;
        public bool IsOpen => _state is OpenState;
        public bool IsHalfOpen => _state is HalfOpenState;

        public CircuitBreaker(CircuitBreakerConfig settings)
        {
            Settings = settings;
            _state = new ClosedState(this);
        }

        public CircuitBreakerState TripToClosedState()
        {
            _state = new ClosedState(this);
            NotifyStateChange(_state);
            return _state;
        }

        public CircuitBreakerState TripToOpenState()
        {
            _state = new OpenState(this);
            NotifyStateChange(_state);
            return _state;
        }

        public CircuitBreakerState TripToHalfOpenState()
        {
            _state = new HalfOpenState(this);
            NotifyStateChange(_state);
            return _state;
        }

        public void Execute(Action action) => _state.Execute(action.ThrowIfNull(nameof(action)));

        public T Execute<T>(Func<T> func) => _state.Execute(func.ThrowIfNull(nameof(func)));

        public Task ExecuteAsync(Func<Task> func) => _state.ExecuteAsync(func.ThrowIfNull(nameof(func)));

        public Task<T> ExecuteAsync<T>(Func<Task<T>> func) => _state.ExecuteAsync(func.ThrowIfNull(nameof(func)));

        private void NotifyStateChange(CircuitBreakerState state) => OnStateChange?.Invoke(state);
    }

    public class CircuitBreakerOpenException : Exception { }
    public class CircuitBreakerTimeoutException : Exception { }
}