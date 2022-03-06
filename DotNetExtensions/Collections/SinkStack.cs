using System.Collections;
using System.Collections.Generic;

namespace DotNetExtensions.Collections;

/// <summary>
/// Represents the collection with fixed size. When stack is full and new element is being added, the last will sink
/// </summary>
public class SinkStack<T> : IReadOnlyCollection<T>
{
    private T[] _array;

    /// <inheritdoc/> 
    public int Count { get; private set; }
    /// <summary>
    /// Determine if stack contains any elements
    /// </summary>
    public bool IsEmpty => Count == 0;
    /// <summary>
    /// Given maximum items count;
    /// </summary>
    public int MaxCount { get; }

    /// <summary>
    /// Creates an SinkStack with given <see cref="MaxCount"/> size.
    /// </summary>
    /// <param name="maxCount">The value must be higher than 1.</param>
    /// <exception cref="ArgumentException">Throws when given count is lower or equal zero</exception>
    public SinkStack(int maxCount)
    {
        if (maxCount <= 0)
            throw new ArgumentException($"Stack maxCount must be positive number. Provided value of {maxCount}");

        _array = new T[maxCount];
        this.MaxCount = maxCount;
    }

    /// <summary>
    /// Creates an SinkStack with given <see cref="MaxCount"/> size and fill stack with given collection
    /// </summary>
    /// <param name="other">Collection which will be transformed into stack</param>
    /// <param name="maxSize">Maximum size of stack</param>
    /// <exception cref="ArgumentException">Throws when given count is lower or equal zero or provided collection is null</exception>
    /// <exception cref="InvalidOperationException">Throws when given collection`s count is higher than provided maximum size</exception>
    public SinkStack(IReadOnlyCollection<T> other, int maxSize)
        : this(maxSize)
    {
        if (other == null)
            throw new ArgumentException("Given collection is null");

        if (other.Count > maxSize)
            throw new InvalidOperationException(
                $"Given collection`s size ({other.Count}) is higher than declared maximum size ({maxSize})");

        var tempArray = new T[MaxCount];
        Array.Copy(other.ToArray(), 0, tempArray, 0, other.Count);
        _array = tempArray;

        this.Count = other.Count;
    }

    /// <summary>
    /// Creates an SinkStack with based on given collection. The <see cref="MaxCount"/> will be size of given collection
    /// </summary>
    /// <param name="other">Collection which will be transformed into stack</param>
    /// <exception cref="ArgumentException">Throws when given collection is null</exception>
    public SinkStack(IReadOnlyCollection<T> other)
        : this(other, other?.Count ?? throw new ArgumentException("Given collection is null"))
    {

    }

    /// <summary>
    /// Add item to the stack
    /// </summary>
    public void Push(T item)
    {
        if (Count == MaxCount)
        {
            var tempArray = new T[MaxCount];
            Array.Copy(_array, 1, tempArray, 0, Count - 1);
            _array = tempArray;
            _array[Count - 1] = item;
        }
        else
        {
            _array[Count] = item;
            Count++;
        }
    }

    /// <summary>
    /// Add multiple items into stack
    /// </summary>
    public void PushRange(params T[] items)
    {
        this.PushRange(items.AsEnumerable());
    }

    /// <summary>
    /// Add multiple items into stack
    /// </summary>
    public void PushRange(IEnumerable<T>? items)
    {
        if (items == null)
            return;

        foreach (var item in items)
        {
            this.Push(item);
        }
    }

    /// <summary>
    /// Takes out current top item from stack.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Throws when stack is empty</exception>
    public T Pop()
    {
        if (this.IsEmpty)
            throw new InvalidOperationException("Stack is empty");

        var result = _array[Count - 1];
        _array[Count - 1] = default;

        Count--;

        return result;
    }

    /// <summary>
    /// Try take out current top item from stack
    /// </summary>
    public bool TryPop(out T? value)
    {
        if (Count == 0)
        {
            value = default;

            return false;
        }

        value = this.Pop();

        return true;
    }

    /// <summary>
    /// Try take out current top item from stack else returns given fallback value
    /// </summary>
    /// <param name="default">The value which will be returned in case stack is empty</param>
    public T? PopOrDefault(T? @default)
    {
        return TryPop(out var value) ? value : @default;
    }

    /// <summary>
    /// Get the current top item from stack.
    /// The item remain in stack.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Throw when stack is empty</exception>
    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Stack is empty");

        return _array[Count - 1];
    }

    /// <summary>
    /// Try to take current top item from stack.
    /// </summary>
    public bool TryPeek(out T? value)
    {
        if (Count == 0)
        {
            value = default;

            return false;
        }

        value = this.Peek();

        return true;
    }

    /// <summary>
    /// Try to get current top item from stack else return given fallback value
    /// </summary>
    public T? PeekOrDefault(T? @default)
    {
        return TryPeek(out var value) ? value : @default;
    }

    /// <summary>
    /// Determine if stack contains given item.
    /// Uses default <see cref="EqualityComparer{T}"/> 
    /// </summary>
    public bool Contains(T other)
    {
        return this.Contains(other, EqualityComparer<T>.Default);
    }

    /// <summary>
    /// Determine if stack contains given item with given <see cref="IEqualityComparer{T}"/>
    /// </summary>
    public bool Contains(T other, IEqualityComparer<T> comparer)
    {
        return Count != 0 && _array.Any(item => comparer.Equals(item, other));
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct Enumerator<T> : IEnumerator<T>
    {
        private T? _current;

        public T Current
        {
            get
            {
                if (_index < 0)
                    throw new InvalidOperationException("Enumeration not started or not ended");
                return _current!;
            }
        }

        private readonly SinkStack<T> _stack;
        private int _index;

        public Enumerator(SinkStack<T> stack)
        {
            _stack = stack;
            _index = -1;
            _current = default;
        }

        public bool MoveNext()
        {
            if (_index == _stack.Count)
                return false;

            if (_index == -1)
                _index = 0;

            _current = _stack._array[_index];

            _index++;

            return true;
        }

        public void Reset()
        {
            _index = -1;
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _index = -1;
        }
    }
}