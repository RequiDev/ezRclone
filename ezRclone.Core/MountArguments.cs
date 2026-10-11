using System.Text;

namespace ezRclone.Core;

public static class MountArguments
{
    public static IReadOnlyList<string> VfsCacheModes { get; } = ["off", "minimal", "writes", "full"];

    /// <summary>
    /// Builds the <c>rclone mount</c> argument list for a remote. Arguments are returned unquoted, meant for
    /// <see cref="System.Diagnostics.ProcessStartInfo.ArgumentList"/>.
    /// </summary>
    public static IReadOnlyList<string> Build(Mountable mountable)
    {
        var driveLetter = DriveLetters.Normalize(mountable.DriveLetter)
                          ?? throw new ArgumentException($"'{mountable.DriveLetter}' is not a valid drive letter.", nameof(mountable));

        var arguments = new List<string> { "mount", $"{mountable.Remote}:{mountable.Path}", $"{driveLetter}:" };

        if (!string.IsNullOrWhiteSpace(mountable.Name))
            arguments.AddRange(["--volname", mountable.Name]);

        if (mountable.NetworkDrive)
            arguments.Add("--network-mode");

        if (!string.IsNullOrWhiteSpace(mountable.VfsCacheMode))
            arguments.AddRange(["--vfs-cache-mode", mountable.VfsCacheMode]);

        arguments.AddRange(Split(mountable.ExtraArguments));
        return arguments;
    }

    /// <summary>
    /// Splits a command line on whitespace. Double quotes group text containing spaces and are removed.
    /// </summary>
    public static IReadOnlyList<string> Split(string? commandLine)
    {
        var arguments = new List<string>();
        if (string.IsNullOrWhiteSpace(commandLine))
            return arguments;

        var current = new StringBuilder();
        var inQuotes = false;
        var hasArgument = false;

        foreach (var c in commandLine)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                hasArgument = true;
            }
            else if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (hasArgument)
                    arguments.Add(current.ToString());

                current.Clear();
                hasArgument = false;
            }
            else
            {
                current.Append(c);
                hasArgument = true;
            }
        }

        if (hasArgument)
            arguments.Add(current.ToString());

        return arguments;
    }
}
