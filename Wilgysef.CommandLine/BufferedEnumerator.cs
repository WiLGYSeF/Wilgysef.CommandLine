using System.Collections;

namespace Wilgysef.CommandLine;

internal class BufferedEnumerator<T> : IEnumerator<T>
{
    private readonly IEnumerator<T> _enumerator;
    private readonly Queue<T> _queue;
    private readonly int _bufferLimit;

    private int _queueIndex = -1;
    private int _rollbackCount = 0;

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

    public T Current => _queueIndex != -1
        ? _queue.ElementAt(_queueIndex)
        : _enumerator.Current;

    object IEnumerator.Current => Current;

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

    public void Rollback(int count)
    {
        var oldRollbackCount = _rollbackCount;
        if (TryRollback(count) != count)
        {
            _rollbackCount = oldRollbackCount;
            throw new Exception($"Could not rollback {count} items");
        }
    }

    public void Dispose()
    {
        _enumerator.Dispose();
    }

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

    public void Reset()
    {
        _queue.Clear();
        _queueIndex = -1;
        _rollbackCount = 0;
        _enumerator.Reset();
    }
}
