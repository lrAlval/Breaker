using System.Threading;

namespace Breaker.Utils
{
    public class AtomicReference<T> where T : class
    {
        public static AtomicReference<T> Default => new AtomicReference<T>();

        private T _value;

        public AtomicReference()
            : this(default)
        {
        }

        public AtomicReference(T value) => _value = value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        public bool CompareAndSet(T expected, T updated) => Interlocked.CompareExchange(ref _value, updated, expected) == expected;

        public T GetAndSet(T value) => Interlocked.Exchange(ref _value, value);
    }
}