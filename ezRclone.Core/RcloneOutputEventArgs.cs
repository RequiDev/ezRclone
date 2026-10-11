namespace ezRclone.Core;

public sealed class RcloneOutputEventArgs(string remote, string line) : EventArgs
{
    public string Remote { get; } = remote;
    public string Line { get; } = line;
    public DateTime Timestamp { get; } = DateTime.Now;
}
