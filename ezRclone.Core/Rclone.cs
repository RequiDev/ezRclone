using System.ComponentModel;
using System.Diagnostics;

namespace ezRclone.Core;

/// <summary>
/// Runs short lived rclone commands. An empty directory means rclone is resolved from PATH.
/// </summary>
public sealed class Rclone
{
    private static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(30);

    public Rclone(string directory)
    {
        ExecutablePath = ResolveExecutable(directory);
    }

    private Rclone(string executablePath, bool _)
    {
        ExecutablePath = executablePath;
    }

    public static string ExecutableName => OperatingSystem.IsWindows() ? "rclone.exe" : "rclone";

    public string ExecutablePath { get; }

    public static string ResolveExecutable(string directory) =>
        string.IsNullOrWhiteSpace(directory) ? ExecutableName : Path.Combine(directory, ExecutableName);

    internal static Rclone FromExecutable(string executablePath) => new(executablePath, true);

    public async Task<IReadOnlyList<RemoteInfo>> ListRemotesAsync(CancellationToken cancellationToken = default)
    {
        return ParseListRemotes(await RunAsync(["listremotes", "--long"], cancellationToken));
    }

    public async Task<string> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        var output = await RunAsync(["version"], cancellationToken);
        return output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault() ?? string.Empty;
    }

    /// <summary>
    /// Parses the output of <c>rclone listremotes --long</c>, which prints one <c>name:  type</c> line per remote.
    /// </summary>
    public static IReadOnlyList<RemoteInfo> ParseListRemotes(string output)
    {
        var remotes = new List<RemoteInfo>();
        foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            // Remote names can contain spaces but never colons.
            var separator = line.IndexOf(':');
            if (separator <= 0)
                continue;

            var name = line[..separator];
            var type = line[(separator + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
            remotes.Add(new RemoteInfo(name, type));
        }

        return remotes;
    }

    private async Task<string> RunAsync(string[] arguments, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo(ExecutablePath)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (var argument in arguments)
            psi.ArgumentList.Add(argument);

        using var process = new Process { StartInfo = psi };
        try
        {
            process.Start();
        }
        catch (Win32Exception e)
        {
            throw new RcloneException($"Could not start {ExecutablePath}: {e.Message}", e);
        }

        // Never let rclone wait for input, e.g. the password prompt of an encrypted config.
        process.StandardInput.Close();

        var stdout = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderr = process.StandardError.ReadToEndAsync(cancellationToken);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(CommandTimeout);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(true);
            cancellationToken.ThrowIfCancellationRequested();
            throw new RcloneException($"rclone {string.Join(' ', arguments)} timed out.");
        }

        if (process.ExitCode != 0)
        {
            var error = (await stderr).Trim();
            throw new RcloneException(string.IsNullOrEmpty(error)
                ? $"rclone {string.Join(' ', arguments)} failed with exit code {process.ExitCode}."
                : error);
        }

        return await stdout;
    }
}
