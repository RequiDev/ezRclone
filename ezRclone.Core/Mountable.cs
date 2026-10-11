namespace ezRclone.Core;

public class Mountable
{
    public string Remote { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string DriveLetter { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool AutoMount { get; set; }
    public bool NetworkDrive { get; set; }
    public string VfsCacheMode { get; set; } = "writes";
    public string ExtraArguments { get; set; } = string.Empty;
}
