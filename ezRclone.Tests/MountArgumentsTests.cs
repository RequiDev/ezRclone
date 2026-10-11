using ezRclone.Core;

namespace ezRclone.Tests;

public class MountArgumentsTests
{
    [Fact]
    public void Build_MinimalMountable()
    {
        var arguments = MountArguments.Build(new Mountable { Remote = "gdrive", DriveLetter = "g", VfsCacheMode = "" });

        Assert.Equal(["mount", "gdrive:", "G:"], arguments);
    }

    [Fact]
    public void Build_AllOptions()
    {
        var arguments = MountArguments.Build(new Mountable
        {
            Remote = "my nas",
            Path = "media/movies",
            DriveLetter = "M:",
            Name = "Movies on NAS",
            NetworkDrive = true,
            VfsCacheMode = "full",
            ExtraArguments = """--vfs-cache-max-size 10G --log-file "C:\logs\nas log.txt" """
        });

        Assert.Equal(
        [
            "mount", "my nas:media/movies", "M:",
            "--volname", "Movies on NAS",
            "--network-mode",
            "--vfs-cache-mode", "full",
            "--vfs-cache-max-size", "10G",
            "--log-file", @"C:\logs\nas log.txt"
        ], arguments);
    }

    [Fact]
    public void Build_InvalidDriveLetter_Throws()
    {
        Assert.Throws<ArgumentException>(() => MountArguments.Build(new Mountable { Remote = "gdrive" }));
    }

    [Theory]
    [InlineData("", new string[0])]
    [InlineData("  --a   b ", new[] { "--a", "b" })]
    [InlineData("--x=\"a b\" c", new[] { "--x=a b", "c" })]
    [InlineData("\"\"", new[] { "" })]
    public void Split_HandlesWhitespaceAndQuotes(string input, string[] expected)
    {
        Assert.Equal(expected, MountArguments.Split(input));
    }
}
