using ezRclone.Core;

namespace ezRclone.Tests;

public class DriveLettersTests
{
    [Theory]
    [InlineData("g", "G")]
    [InlineData("G:", "G")]
    [InlineData(@" G:\ ", "G")]
    [InlineData("", null)]
    [InlineData("GG", null)]
    [InlineData("1", null)]
    [InlineData(null, null)]
    public void Normalize_AcceptsCommonForms(string? input, string? expected)
    {
        Assert.Equal(expected, DriveLetters.Normalize(input));
    }

    [Fact]
    public void GetAvailable_ExcludesUsedLettersButKeepsRequestedOne()
    {
        var used = new HashSet<string> { "C", "D", "G" };

        var available = DriveLetters.GetAvailable(used, keep: "g:");

        Assert.DoesNotContain("C", available);
        Assert.DoesNotContain("D", available);
        Assert.Contains("G", available);
        Assert.Contains("Z", available);
        Assert.Equal(24, available.Count);
    }

    [Fact]
    public void Root_ReturnsDriveRoot()
    {
        Assert.Equal(@"G:\", DriveLetters.Root("g"));
    }
}
