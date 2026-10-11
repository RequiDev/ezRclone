using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ezRclone.Core;

/// <summary>
/// Starts, stops and watches <c>rclone mount</c> processes. Events are raised on background threads.
/// </summary>
public sealed partial class MountManager
{
    private const int KeptOutputLines = 20;

    private readonly object _lock = new();
    private readonly Dictionary<string, MountProcess> _processes = new();
    private readonly Dictionary<string, MountState> _states = new();

    public string RcloneDirectory { get; set; } = string.Empty;

    /// <summary>
    /// How long to wait for the drive to show up before a still running mount is considered mounted anyway.
    /// </summary>
    public TimeSpan ReadyTimeout { get; set; } = TimeSpan.FromSeconds(15);

    public Func<Mountable, bool> IsDriveReady { get; set; } =
        mountable => Directory.Exists(DriveLetters.Root(mountable.DriveLetter));

    internal string? ExecutableOverride { get; set; }

    public event EventHandler<MountState>? StateChanged;
    public event EventHandler<RcloneOutputEventArgs>? OutputReceived;

    public MountState GetState(string remote)
    {
        lock (_lock)
        {
            return _states.TryGetValue(remote, out var state) ? state : new MountState(remote, MountStatus.Unmounted);
        }
    }

    public bool IsActive(string remote) => GetState(remote).IsActive;

    public IReadOnlyList<string> GetActiveRemotes()
    {
        lock (_lock)
        {
            return _processes.Keys.ToArray();
        }
    }

    public async Task<MountState> MountAsync(Mountable mountable)
    {
        var remote = mountable.Remote;
        var driveLetter = DriveLetters.Normalize(mountable.DriveLetter);
        if (driveLetter == null)
            return Publish(new MountState(remote, MountStatus.Failed, "No drive letter set."));

        if (IsActive(remote))
            return GetState(remote);

        if (IsDriveReady(mountable))
            return Publish(new MountState(remote, MountStatus.Failed, $"Drive {driveLetter}: is already in use."));

        var psi = new ProcessStartInfo(ExecutableOverride ?? Rclone.ResolveExecutable(RcloneDirectory))
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        foreach (var argument in MountArguments.Build(mountable))
            psi.ArgumentList.Add(argument);

        var mount = new MountProcess(remote, new Process { StartInfo = psi, EnableRaisingEvents = true });
        lock (_lock)
        {
            if (!_processes.TryAdd(remote, mount))
                return GetState(remote);
        }

        mount.Process.OutputDataReceived += (_, e) => OnOutput(mount, e.Data);
        mount.Process.ErrorDataReceived += (_, e) => OnOutput(mount, e.Data);
        mount.Process.Exited += (_, _) => mount.Exited.TrySetResult();

        Publish(new MountState(remote, MountStatus.Mounting));
        try
        {
            mount.Process.Start();
        }
        catch (Win32Exception e)
        {
            lock (_lock)
            {
                _processes.Remove(remote);
            }

            mount.Process.Dispose();
            return Publish(new MountState(remote, MountStatus.Failed, $"Could not start {psi.FileName}: {e.Message}"));
        }

        mount.Process.BeginOutputReadLine();
        mount.Process.BeginErrorReadLine();
        _ = MonitorAsync(mount);

        var stopwatch = Stopwatch.StartNew();
        while (!mount.Finished.Task.IsCompleted && !IsDriveReady(mountable) && stopwatch.Elapsed < ReadyTimeout)
        {
            await Task.WhenAny(mount.Finished.Task, Task.Delay(250)).ConfigureAwait(false);
        }

        MountState? mounted = null;
        lock (_lock)
        {
            if (_processes.TryGetValue(remote, out var current) && current == mount && !mount.Stopping)
                _states[remote] = mounted = new MountState(remote, MountStatus.Mounted);
        }

        if (mounted != null)
            StateChanged?.Invoke(this, mounted);

        return GetState(remote);
    }

    /// <summary>
    /// Takes over an rclone mount process that was started outside of this manager, e.g. by a previous session.
    /// </summary>
    public void Adopt(string remote, Process process)
    {
        var mount = new MountProcess(remote, process, redirected: false);
        lock (_lock)
        {
            if (!_processes.TryAdd(remote, mount))
                return;
        }

        process.Exited += (_, _) => mount.Exited.TrySetResult();
        try
        {
            process.EnableRaisingEvents = true;
        }
        catch (Exception e) when (e is InvalidOperationException or Win32Exception)
        {
            lock (_lock)
            {
                _processes.Remove(remote);
            }

            return;
        }

        Publish(new MountState(remote, MountStatus.Mounted));
        _ = MonitorAsync(mount);
    }

    public async Task UnmountAsync(string remote)
    {
        MountProcess? mount;
        MountState state;
        lock (_lock)
        {
            if (!_processes.TryGetValue(remote, out mount))
            {
                // Nothing is running, this only clears a previous failure.
                if (_states.TryGetValue(remote, out var previous) && previous.Status == MountStatus.Failed)
                    _states[remote] = state = new MountState(remote, MountStatus.Unmounted);
                else
                    return;
            }
            else
            {
                mount.Stopping = true;
                _states[remote] = state = new MountState(remote, MountStatus.Unmounting);
            }
        }

        StateChanged?.Invoke(this, state);
        if (mount == null)
            return;

        try
        {
            mount.Process.Kill(true);
        }
        catch (InvalidOperationException)
        {
            // Already exited, the monitor takes care of the rest.
        }
        catch (Win32Exception e)
        {
            lock (_lock)
            {
                mount.Stopping = false;
            }

            Publish(new MountState(remote, MountStatus.Mounted, $"Could not stop rclone: {e.Message}"));
            return;
        }

        await Task.WhenAny(mount.Finished.Task, Task.Delay(TimeSpan.FromSeconds(10))).ConfigureAwait(false);
    }

    public Task UnmountAllAsync() => Task.WhenAll(GetActiveRemotes().Select(UnmountAsync));

    private async Task MonitorAsync(MountProcess mount)
    {
        await mount.Exited.Task.ConfigureAwait(false);
        try
        {
            if (mount.Redirected)
            {
                // Give the output readers a moment to drain so the failure message contains rclone's last words.
                using var drain = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                try
                {
                    await mount.Process.WaitForExitAsync(drain.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            MountState state;
            lock (_lock)
            {
                _processes.Remove(mount.Remote);
                state = mount.Stopping
                    ? new MountState(mount.Remote, MountStatus.Unmounted)
                    : new MountState(mount.Remote, MountStatus.Failed, DescribeExit(mount));
                _states[mount.Remote] = state;
            }

            mount.Process.Dispose();
            StateChanged?.Invoke(this, state);
        }
        finally
        {
            mount.Finished.TrySetResult();
        }
    }

    private void OnOutput(MountProcess mount, string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        mount.AddOutput(line);
        OutputReceived?.Invoke(this, new RcloneOutputEventArgs(mount.Remote, line));
    }

    private MountState Publish(MountState state)
    {
        lock (_lock)
        {
            _states[state.Remote] = state;
        }

        StateChanged?.Invoke(this, state);
        return state;
    }

    internal static string DescribeExit(int? exitCode, IReadOnlyList<string> output)
    {
        var message = exitCode is { } code ? $"rclone exited with code {code}." : "rclone exited.";

        // Prefer rclone's own error over whatever it logged last.
        var error = output.LastOrDefault(line => line.Contains("CRITICAL") || line.Contains("ERROR"))
                    ?? output.LastOrDefault();
        return error == null ? message : $"{message} {LogPrefix().Replace(error, string.Empty)}";
    }

    private static string DescribeExit(MountProcess mount)
    {
        int? exitCode;
        try
        {
            exitCode = mount.Process.ExitCode;
        }
        catch (Exception e) when (e is InvalidOperationException or Win32Exception)
        {
            exitCode = null;
        }

        return DescribeExit(exitCode, mount.GetOutput());
    }

    // rclone prefixes log lines with "2024/10/11 12:34:56 ".
    [GeneratedRegex(@"^\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2} ")]
    private static partial Regex LogPrefix();

    private sealed class MountProcess(string remote, Process process, bool redirected = true)
    {
        private readonly Queue<string> _output = new();

        public string Remote { get; } = remote;
        public Process Process { get; } = process;
        public bool Redirected { get; } = redirected;

        /// <summary>
        /// Set when the exit was requested, guarded by the manager's lock.
        /// </summary>
        public bool Stopping { get; set; }

        public TaskCompletionSource Exited { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Finished { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void AddOutput(string line)
        {
            lock (_output)
            {
                _output.Enqueue(line);
                if (_output.Count > KeptOutputLines)
                    _output.Dequeue();
            }
        }

        public IReadOnlyList<string> GetOutput()
        {
            lock (_output)
            {
                return _output.ToArray();
            }
        }
    }
}
