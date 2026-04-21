using System.Collections.Concurrent;

namespace ContosoDashboard.Services;

public class DocumentQueue
{
    private readonly ConcurrentQueue<string> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void Enqueue(string message)
    {
        _queue.Enqueue(message);
        _signal.Release();
    }

    public async Task<string?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        await _signal.WaitAsync(cancellationToken);
        if (_queue.TryDequeue(out var msg))
            return msg;
        return null;
    }

    public int Count => _queue.Count;
}
