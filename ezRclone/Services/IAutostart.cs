namespace ezRclone.Services;

public interface IAutostart
{
    bool IsEnabled { get; }

    /// <summary>
    /// Whether ezRclone starts minimized to the tray when launched at sign in.
    /// </summary>
    bool StartsHidden { get; }

    void Set(bool enabled, bool hidden);
}
