using ezRclone.Core;

namespace ezRclone.Tests;

public class RcloneTests
{
    [Fact]
    public void ParseListRemotes_ReadsNamesAndTypes()
    {
        const string output = "gdrive:      drive\r\nmy nas:      sftp   Home NAS\n\nsecret:      crypt\n";

        var remotes = Rclone.ParseListRemotes(output);

        Assert.Equal(
            [new RemoteInfo("gdrive", "drive"), new RemoteInfo("my nas", "sftp"), new RemoteInfo("secret", "crypt")],
            remotes);
    }

    [Fact]
    public void ParseListRemotes_AcceptsShortFormat()
    {
        var remotes = Rclone.ParseListRemotes("gdrive:\nonedrive:\n");

        Assert.Equal([new RemoteInfo("gdrive", ""), new RemoteInfo("onedrive", "")], remotes);
    }

    [Fact]
    public void ParseListRemotes_IgnoresNoise()
    {
        Assert.Empty(Rclone.ParseListRemotes("\n   \n:nameless\n"));
    }

    [Fact]
    public void ExecutablePath_UsesPathWhenDirectoryIsEmpty()
    {
        Assert.Equal(Rclone.ExecutableName, new Rclone("").ExecutablePath);
        Assert.Equal(Path.Combine("tools", Rclone.ExecutableName), new Rclone("tools").ExecutablePath);
    }

    [Fact]
    public async Task ListRemotesAsync_MissingExecutable_ThrowsRcloneException()
    {
        using var dir = new TempDirectory();

        await Assert.ThrowsAsync<RcloneException>(() => new Rclone(dir.Path).ListRemotesAsync());
    }

    [Fact]
    public async Task ListRemotesAsync_ParsesProcessOutput()
    {
        using var dir = new TempDirectory();
        var rclone = Rclone.FromExecutable(FakeRclone.Create(dir, stdout: ["gdrive:      drive", "nas:         sftp"]));

        var remotes = await rclone.ListRemotesAsync();

        Assert.Equal([new RemoteInfo("gdrive", "drive"), new RemoteInfo("nas", "sftp")], remotes);
    }

    [Fact]
    public async Task GetVersionAsync_ReturnsFirstLine()
    {
        using var dir = new TempDirectory();
        var rclone = Rclone.FromExecutable(FakeRclone.Create(dir, stdout: ["rclone v1.68.1", "- os/version: test"]));

        Assert.Equal("rclone v1.68.1", await rclone.GetVersionAsync());
    }

    [Fact]
    public async Task ListRemotesAsync_NonZeroExit_ThrowsWithStandardError()
    {
        using var dir = new TempDirectory();
        var rclone = Rclone.FromExecutable(FakeRclone.Create(dir, stderr: ["config is encrypted"], exitCode: 1));

        var exception = await Assert.ThrowsAsync<RcloneException>(() => rclone.ListRemotesAsync());
        Assert.Equal("config is encrypted", exception.Message);
    }
}
