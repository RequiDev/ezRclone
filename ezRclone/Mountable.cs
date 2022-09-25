namespace ezRclone;

public class Mountable
{
    public string Remote { get; set; } = string.Empty;
    public string Path { get; set; } = "/";
    public string DriveLetter { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool AutoMount { get; set; } = false;
    public bool NetworkDrive { get; set; } = false;
}