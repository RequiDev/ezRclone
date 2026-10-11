namespace ezRclone.ViewModels;

public sealed record LogEntry(DateTime Timestamp, string Remote, string Message, bool IsError)
{
    public override string ToString() => $"{Timestamp:HH:mm:ss} [{Remote}] {Message}";
}
