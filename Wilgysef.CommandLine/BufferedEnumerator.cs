using System.Collections;

namespace Wilgysef.CommandLine;

/// <summary>
/// Enumerator that keeps a buffered history of past items for rolling back the current enumerator value.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
internal class BufferedEnumerator<T> : IEnumerator<T>
{
    private readonly IEnumerator<T> _enumerator;
    private readonly Queue<T> _queue;
    private readonly int _bufferLimit;

    private int _queueIndex = -1;
    private int _rollbackCount = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferedEnumerator{T}"/> class.
    /// </summary>
    /// <param name="enumerator">Enumerator.</param>
    /// <param name="buffer">Number of previous items to buffer.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="buffer"/> is 0.</exception>
    public BufferedEnumerator(IEnumerator<T> enumerator, int buffer)
    {
        if (buffer <= 0)
        {
            throw new ArgumentException("Buffer size must be greater than 0", nameof(buffer));
        }

        _enumerator = enumerator;
        _bufferLimit = buffer;
        _queue = new Queue<T>(_bufferLimit);
    }

    /// <inheritdoc/>
    public T Current => _queueIndex != -1
        ? _queue.ElementAt(_queueIndex)
        : _enumerator.Current;

    /// <inheritdoc/>
    object? IEnumerator.Current => Current;

    /// <summary>
    /// Tries to rollback <see cref="Current"/> to a previous value.
    /// </summary>
    /// <param name="count">Number of values to rollback to.</param>
    /// <returns>Number of values rolled back.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="count"/> is less than or equal to 0.</exception>
    public int TryRollback(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Rollback count should be greater than 0", nameof(count));
        }

        if (_rollbackCount + count > _queueIndex + 1)
        {
            var old = _rollbackCount;
            _rollbackCount = _queueIndex + 1;
            return _rollbackCount - old;
        }
        else
        {
            _rollbackCount += count;
            return count;
        }
    }

    /// <summary>
    /// Rolls back <see cref="Current"/> to a previous value.
    /// </summary>
    /// <param name="count">Number of values to rollback.</param>
    /// <exception cref="Exception">Thrown if <paramref name="count"/> exceeds the number of values buffered.</exception>
    public void Rollback(int count)
    {
        var oldRollbackCount = _rollbackCount;
        if (TryRollback(count) != count)
        {
            _rollbackCount = oldRollbackCount;
            throw new Exception($"Could not rollback {count} items");
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _enumerator.Dispose();
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        if (_rollbackCount != 0)
        {
            _queueIndex -= _rollbackCount - 1;
            _rollbackCount = 0;
            return true;
        }

        if (_queueIndex >= 0 && _queueIndex != _queue.Count - 1)
        {
            _queueIndex++;
            return true;
        }

        if (_enumerator.MoveNext())
        {
            if (_queue.Count == _bufferLimit)
            {
                _queue.Dequeue();
                _queueIndex--;
            }

            _queue.Enqueue(_enumerator.Current);
            _queueIndex++;
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _queue.Clear();
        _queueIndex = -1;
        _rollbackCount = 0;
        _enumerator.Reset();
    }
}
