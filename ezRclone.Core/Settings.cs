namespace ezRclone.Core;

public class Settings
{
    public string RclonePath { get; set; } = string.Empty;

    /// <summary>
    /// Stop all mounts when ezRclone exits. Otherwise they keep running and are picked up again on the next start.
    /// </summary>
    public bool UnmountOnExit { get; set; }
    public List<Mountable> Mountables { get; set; } = new();
}
