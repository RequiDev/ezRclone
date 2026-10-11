using System.Diagnostics;
using System.Management;
using ezRclone.Core;

namespace ezRclone.Services;

/// <summary>
/// Finds rclone mounts that are still running, e.g. from before ezRclone was restarted, and hands them to the
/// <see cref="MountManager"/>.
/// </summary>
public static class ExistingMounts
{
    public static void Adopt(MountManager mountManager, IReadOnlyCollection<string> remotes)
    {
        foreach (var process in Process.GetProcessesByName("rclone"))
        {
            var remote = MountArguments.FindMountedRemote(GetCommandLine(process), remotes);
            if (remote != null && !mountManager.IsActive(remote))
                mountManager.Adopt(remote, process);
            else
                process.Dispose();
        }
    }

    private static string GetCommandLine(Process process)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id);
            using var objects = searcher.Get();
            return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString() ?? string.Empty;
        }
        catch (ManagementException)
        {
            return string.Empty;
        }
    }
}
