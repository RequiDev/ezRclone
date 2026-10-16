namespace ezRclone.Core;

public enum MountStatus
{
    Unmounted,
    Mounting,
    Mounted,
    Unmounting,
    Failed
}

public sealed record MountState(string Remote, MountStatus Status, string? Message = null)
{
    /// <summary>
    /// True while an rclone process is running for the remote.
    /// </summary>
    public bool IsActive => Status is MountStatus.Mounting or MountStatus.Mounted or MountStatus.Unmounting;
}
