namespace ezRclone.Core;

public static class DriveLetters
{
    public static IReadOnlyList<string> All { get; } = Enumerable.Range('A', 26).Select(c => ((char)c).ToString()).ToArray();

    /// <summary>
    /// Turns user input such as <c>g</c>, <c>G:</c> or <c>G:\</c> into <c>G</c>. Returns null for anything else.
    /// </summary>
    public static string? Normalize(string? value)
    {
        var trimmed = value?.Trim().TrimEnd('\\', '/').TrimEnd(':');
        if (trimmed is not { Length: 1 } || !char.IsAsciiLetter(trimmed[0]))
            return null;

        return char.ToUpperInvariant(trimmed[0]).ToString();
    }

    public static string Root(string driveLetter) => $"{Normalize(driveLetter)}:\\";

    /// <summary>
    /// Drive letters currently used by the system, e.g. disks, DVD drives and mapped network drives.
    /// </summary>
    public static IReadOnlySet<string> GetUsed()
    {
        if (!OperatingSystem.IsWindows())
            return new HashSet<string>();

        return DriveInfo.GetDrives()
            .Select(drive => Normalize(drive.Name))
            .OfType<string>()
            .ToHashSet();
    }

    /// <summary>
    /// Letters that are free to mount on. <paramref name="keep"/> stays in the list even when it is in use, so a
    /// remote that is currently mounted keeps showing its own letter.
    /// </summary>
    public static IReadOnlyList<string> GetAvailable(IReadOnlySet<string> used, string? keep = null)
    {
        var kept = Normalize(keep);
        return All.Where(letter => letter == kept || !used.Contains(letter)).ToArray();
    }
}
