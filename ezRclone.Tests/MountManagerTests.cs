using System.Diagnostics;
using ezRclone.Core;

namespace ezRclone.Tests;

public class MountManagerTests
{
    private static readonly Mountable Remote = new() { Remote = "gdrive", DriveLetter = "G" };

    private static MountManager CreateManager(string executable) => new()
    {
        ExecutableOverride = executable,
        ReadyTimeout = TimeSpan.FromMilliseconds(300),
        IsDriveReady = _ => false
    };

    [Fact]
    public async Task MountAsync_RunningProcess_IsMountedUntilUnmounted()
    {
        using var dir = new TempDirectory();
        var manager = CreateManager(FakeRclone.Create(dir, runSeconds: 60));
        var states = new List<MountStatus>();
        manager.StateChanged += (_, state) =>
        {
            lock (states)
                states.Add(state.Status);
        };

        var mounted = await manager.MountAsync(Remote);

        Assert.Equal(MountStatus.Mounted, mounted.Status);
        Assert.True(manager.IsActive("gdrive"));
        Assert.Equal(["gdrive"], manager.GetActiveRemotes());

        await manager.UnmountAsync("gdrive");

        Assert.Equal(MountStatus.Unmounted, manager.GetState("gdrive").Status);
        Assert.Empty(manager.GetActiveRemotes());
        lock (states)
        {
            Assert.Equal(
                [MountStatus.Mounting, MountStatus.Mounted, MountStatus.Unmounting, MountStatus.Unmounted],
                states);
        }
    }

    [Fact]
    public async Task MountAsync_ProcessExitsEarly_FailsWithRcloneError()
    {
        using var dir = new TempDirectory();
        var manager = CreateManager(FakeRclone.Create(dir,
            stderr: ["2024/10/11 12:00:00 CRITICAL: Fatal error: cannot find winfsp", "2024/10/11 12:00:00 INFO  : done"],
            exitCode: 7));
        var output = new List<string>();
        manager.OutputReceived += (_, e) =>
        {
            lock (output)
                output.Add(e.Line);
        };

        var state = await manager.MountAsync(Remote);

        Assert.Equal(MountStatus.Failed, state.Status);
        Assert.Equal("rclone exited with code 7. CRITICAL: Fatal error: cannot find winfsp", state.Message);
        Assert.False(manager.IsActive("gdrive"));
        lock (output)
            Assert.Equal(2, output.Count);
    }

    [Fact]
    public async Task MountAsync_ProcessExitsLater_ReportsFailure()
    {
        using var dir = new TempDirectory();
        var manager = CreateManager(FakeRclone.Create(dir, stderr: ["ERROR : connection lost"], exitCode: 1, runSeconds: 2));
        var failed = new TaskCompletionSource<MountState>(TaskCreationOptions.RunContinuationsAsynchronously);
        manager.StateChanged += (_, state) =>
        {
            if (state.Status == MountStatus.Failed)
                failed.TrySetResult(state);
        };

        Assert.Equal(MountStatus.Mounted, (await manager.MountAsync(Remote)).Status);

        var state = await failed.Task.WaitAsync(TimeSpan.FromSeconds(30));
        Assert.Equal("rclone exited with code 1. ERROR : connection lost", state.Message);
    }

    [Fact]
    public async Task MountAsync_DriveAlreadyInUse_FailsWithoutStartingRclone()
    {
        var manager = new MountManager { ExecutableOverride = "does-not-exist", IsDriveReady = _ => true };

        var state = await manager.MountAsync(Remote);

        Assert.Equal(MountStatus.Failed, state.Status);
        Assert.Equal("Drive G: is already in use.", state.Message);
    }

    [Fact]
    public async Task MountAsync_WithoutDriveLetter_Fails()
    {
        var manager = new MountManager { IsDriveReady = _ => false };

        var state = await manager.MountAsync(new Mountable { Remote = "gdrive" });

        Assert.Equal(MountStatus.Failed, state.Status);
        Assert.Equal("No drive letter set.", state.Message);
    }

    [Fact]
    public async Task MountAsync_MissingExecutable_Fails()
    {
        using var dir = new TempDirectory();
        var manager = CreateManager(dir.Combine("missing", Rclone.ExecutableName));

        var state = await manager.MountAsync(Remote);

        Assert.Equal(MountStatus.Failed, state.Status);
        Assert.StartsWith("Could not start", state.Message);
        Assert.Empty(manager.GetActiveRemotes());
    }

    [Fact]
    public async Task UnmountAsync_AfterFailure_ResetsState()
    {
        var manager = new MountManager { IsDriveReady = _ => true };
        await manager.MountAsync(Remote);

        await manager.UnmountAsync("gdrive");

        Assert.Equal(MountStatus.Unmounted, manager.GetState("gdrive").Status);
    }

    [Fact]
    public async Task Adopt_TracksExternalProcessAndUnmountsIt()
    {
        using var dir = new TempDirectory();
        var process = Process.Start(new ProcessStartInfo(FakeRclone.Create(dir, runSeconds: 60)) { CreateNoWindow = true })!;
        var manager = new MountManager();

        manager.Adopt("gdrive", process);

        Assert.Equal(MountStatus.Mounted, manager.GetState("gdrive").Status);

        await manager.UnmountAsync("gdrive");

        Assert.Equal(MountStatus.Unmounted, manager.GetState("gdrive").Status);
    }

    [Fact]
    public void DescribeExit_WithoutOutput()
    {
        Assert.Equal("rclone exited.", MountManager.DescribeExit(null, []));
        Assert.Equal("rclone exited with code 2. last", MountManager.DescribeExit(2, ["first", "last"]));
    }
}
