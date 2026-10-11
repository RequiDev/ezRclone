namespace ezRclone.Core;

public class Settings
{
    public string RclonePath { get; set; } = string.Empty;
    public List<Mountable> Mountables { get; set; } = new();
}
