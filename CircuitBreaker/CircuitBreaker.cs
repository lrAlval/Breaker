using CircuitBreaker.States;
using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreaker
    {
        public CircuitBreakerConfig Settings { get; set; }

        public event Action<CircuitBreakerState> OnStateChange;

        private CircuitBreakerState _state;

        private readonly object _lockObject = new object();

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
            lock (_lockObject)
            {
                _state = new ClosedState(this);
                NotifyStateChange(_state);
                return _state;
            }
        }

        public CircuitBreakerState TripToOpenState()
        {
            lock (_lockObject)
            {
                _state = new OpenState(this);
                NotifyStateChange(_state);
                return _state;
            }
        }

        public CircuitBreakerState TripToHalfOpenState()
        {
            lock (_lockObject)
            {
                _state = new HalfOpenState(this);
                NotifyStateChange(_state);
                return _state;
            }
        }

        public void Execute(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _state.Execute(action);
        }

        public T Execute<T>(Func<T> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return _state.Execute(func);
        }

        public Task ExecuteAsync(Func<Task> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return _state.InvokeAsync(func);
        }

        public Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            return _state.InvokeAsync(func);
        }

        private void NotifyStateChange(CircuitBreakerState state) => this.OnStateChange?.Invoke(state);
    }

    public class CircuitBreakerException : Exception { }
    public class CircuitBreakerExecutionException : CircuitBreakerException { }
    public class CircuitBreakerOpenException : Exception { }
    public class CircuitBreakerTimeoutException : Exception { }
}
