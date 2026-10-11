using ezRclone.Core;

namespace ezRclone.Tests;

public class SettingsStoreTests
{
    [Fact]
    public void Load_WithoutAnyFile_ReturnsDefaults()
    {
        using var dir = new TempDirectory();

        var settings = new SettingsStore(dir.Path).Load();

        Assert.Equal(string.Empty, settings.RclonePath);
        Assert.Empty(settings.Mountables);
    }

    [Fact]
    public void Save_ThenLoad_RoundTripsSettings()
    {
        using var dir = new TempDirectory();
        var store = new SettingsStore(dir.Path);

        store.Save(new Settings
        {
            RclonePath = @"C:\rclone",
            UnmountOnExit = true,
            Mountables = { new Mountable { Remote = "gdrive", DriveLetter = "G", VfsCacheMode = "full", AutoMount = true } }
        });
        var loaded = store.Load();

        Assert.Equal(@"C:\rclone", loaded.RclonePath);
        Assert.True(loaded.UnmountOnExit);
        var mountable = Assert.Single(loaded.Mountables);
        Assert.Equal("gdrive", mountable.Remote);
        Assert.Equal("G", mountable.DriveLetter);
        Assert.Equal("full", mountable.VfsCacheMode);
        Assert.True(mountable.AutoMount);
    }

    [Fact]
    public void Load_MigratesNewestLegacySettingsFile()
    {
        using var dir = new TempDirectory();
        var older = dir.Combine("ezRclone", "1.0.0", SettingsStore.FileName);
        var newer = dir.Combine("ezRclone", "1.0.0+abc123", SettingsStore.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(older)!);
        Directory.CreateDirectory(Path.GetDirectoryName(newer)!);
        File.WriteAllText(older, """{ "RclonePath": "old", "Mountables": [] }""");
        File.WriteAllText(newer, """
            {
              "RclonePath": "C:\\rclone",
              "Mountables": [
                { "Remote": "gdrive", "Path": "/", "DriveLetter": "G", "Name": "Google", "AutoMount": true, "NetworkDrive": false }
              ]
            }
            """);
        File.SetLastWriteTimeUtc(older, DateTime.UtcNow.AddDays(-1));

        var store = new SettingsStore(dir.Path);
        var settings = store.Load();

        Assert.Equal(@"C:\rclone", settings.RclonePath);
        var mountable = Assert.Single(settings.Mountables);
        Assert.Equal("Google", mountable.Name);
        Assert.Equal("writes", mountable.VfsCacheMode); // missing in legacy files, falls back to the old hard coded mode
        Assert.True(File.Exists(store.FilePath));
    }

    [Fact]
    public void Load_InvalidJson_KeepsBackupAndReturnsDefaults()
    {
        using var dir = new TempDirectory();
        var store = new SettingsStore(dir.Path);
        File.WriteAllText(store.FilePath, "{ not json");

        var settings = store.Load();

        Assert.Empty(settings.Mountables);
        Assert.True(File.Exists(dir.Combine("settings.invalid.json")));
    }

    [Fact]
    public void Load_NullMountables_ReturnsEmptyList()
    {
        using var dir = new TempDirectory();
        var store = new SettingsStore(dir.Path);
        File.WriteAllText(store.FilePath, """{ "Mountables": null }""");

        Assert.NotNull(store.Load().Mountables);
    }
}
