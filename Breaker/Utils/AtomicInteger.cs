using System.Threading;

namespace Breaker.Utils
{
    public struct AtomicInteger
    {
        public static AtomicInteger Default => new AtomicInteger();

        private int _value;

        public AtomicInteger(int? value = null) => _value = value ?? 0;

        public int Value
        {
            get => _value;
            set => _value = value;
        }

        public void Reset() => _value = 0;

        public bool CompareAndSet(int expected, int update) => Interlocked.CompareExchange(ref _value, update, expected) == expected;

        public bool CompareAndSet(int update) => Interlocked.CompareExchange(ref _value, update, _value) == _value;

        public bool IncrementAndCompare(int update) => Interlocked.Increment(ref _value) == update;

        public int IncrementAndGet() => Interlocked.Increment(ref _value);

        public int DecrementAndGet() => Interlocked.Decrement(ref _value);

        public int GetAndIncrement() => Interlocked.Increment(ref _value) - 1;

        public int AddAndGet(int value) => Interlocked.Add(ref _value, value);
    }
}