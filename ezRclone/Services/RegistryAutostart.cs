using Microsoft.Win32;

namespace ezRclone.Services;

/// <summary>
/// Stores autostart in the current user's Run key, using the same value as the WinForms version.
/// </summary>
public sealed class RegistryAutostart : IAutostart
{
    private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "ezRclone";
    public const string HiddenArgument = "--hidden";

    public bool IsEnabled => ReadValue() != null;

    public bool StartsHidden => ReadValue()?.Contains(HiddenArgument, StringComparison.OrdinalIgnoreCase) ?? false;

    public void Set(bool enabled, bool hidden)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKey, true);
        if (!enabled)
        {
            key.DeleteValue(ValueName, false);
            return;
        }

        var command = $"\"{Environment.ProcessPath}\"";
        key.SetValue(ValueName, hidden ? $"{command} {HiddenArgument}" : command);
    }

    private static string? ReadValue()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey);
        return key?.GetValue(ValueName) as string;
    }
}
