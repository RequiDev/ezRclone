namespace ezRclone.Services;

/// <summary>
/// Keeps ezRclone to one instance per user session. Later instances can ask the first one to show its window.
/// </summary>
public sealed class SingleInstance : IDisposable
{
    private const string Name = @"Local\ezRclone-8c0e5a52";

    private readonly Mutex _mutex;
    private readonly EventWaitHandle _showRequested;
    private RegisteredWaitHandle? _registration;

    public SingleInstance()
    {
        _mutex = new Mutex(true, Name + ".Instance", out var createdNew);
        IsFirstInstance = createdNew;
        _showRequested = new EventWaitHandle(false, EventResetMode.AutoReset, Name + ".Show");
    }

    public bool IsFirstInstance { get; }

    public void RequestShow() => _showRequested.Set();

    /// <summary>
    /// Calls <paramref name="onShowRequested"/> on a thread pool thread whenever another instance asks to be shown.
    /// </summary>
    public void ListenForShowRequests(Action onShowRequested)
    {
        _registration = ThreadPool.RegisterWaitForSingleObject(_showRequested, (_, _) => onShowRequested(), null,
            Timeout.Infinite, false);
    }

    public void Dispose()
    {
        _registration?.Unregister(null);
        if (IsFirstInstance)
            _mutex.ReleaseMutex();
        _mutex.Dispose();
        _showRequested.Dispose();
    }
}
