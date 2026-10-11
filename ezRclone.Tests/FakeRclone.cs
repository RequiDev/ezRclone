using System.Text;

namespace ezRclone.Tests;

/// <summary>
/// Writes a script that stands in for rclone: it prints the given lines, optionally keeps running and exits with the
/// given code. Uses a batch file on Windows and a shell script elsewhere.
/// </summary>
internal static class FakeRclone
{
    public static string Create(TempDirectory dir, string[]? stdout = null, string[]? stderr = null, int exitCode = 0,
        bool keepRunning = false)
    {
        var script = new StringBuilder();
        string path;

        if (OperatingSystem.IsWindows())
        {
            path = dir.Combine($"rclone-{Guid.NewGuid():N}.cmd");
            script.Append("@echo off\r\n");
            foreach (var line in stdout ?? [])
                script.Append($"echo {line}\r\n");
            foreach (var line in stderr ?? [])
                script.Append($"1>&2 echo {line}\r\n");
            if (keepRunning)
                script.Append("ping -n 120 127.0.0.1 >nul\r\n");
            script.Append($"exit /b {exitCode}\r\n");
            File.WriteAllText(path, script.ToString());
        }
        else
        {
            path = dir.Combine($"rclone-{Guid.NewGuid():N}.sh");
            script.Append("#!/bin/sh\n");
            foreach (var line in stdout ?? [])
                script.Append($"echo '{line}'\n");
            foreach (var line in stderr ?? [])
                script.Append($"echo '{line}' >&2\n");
            if (keepRunning)
                script.Append("sleep 120\n");
            script.Append($"exit {exitCode}\n");
            File.WriteAllText(path, script.ToString());
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }

        return path;
    }
}
