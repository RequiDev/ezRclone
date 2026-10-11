using System.Text.Json;

namespace ezRclone.Core;

/// <summary>
/// Loads and saves <see cref="Settings"/> as JSON. Settings written by the WinForms version, which lived in a
/// per-version sub folder (Application.UserAppDataPath), are migrated on first load.
/// </summary>
public sealed class SettingsStore
{
    public const string FileName = "settings.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _directory;

    public SettingsStore(string directory)
    {
        _directory = directory;
    }

    public static string DefaultDirectory =>
        System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ezRclone");

    public string FilePath => System.IO.Path.Combine(_directory, FileName);

    public Settings Load()
    {
        if (File.Exists(FilePath))
            return Read(FilePath);

        var legacyFile = FindLegacySettingsFile();
        if (legacyFile == null)
            return new Settings();

        var settings = Read(legacyFile);
        Save(settings);
        return settings;
    }

    public void Save(Settings settings)
    {
        Directory.CreateDirectory(_directory);

        // Write to a temporary file first so a crash never leaves a half written settings file behind.
        var tempFile = FilePath + ".tmp";
        File.WriteAllText(tempFile, JsonSerializer.Serialize(settings, JsonOptions));
        File.Move(tempFile, FilePath, true);
    }

    internal string? FindLegacySettingsFile()
    {
        if (!Directory.Exists(_directory))
            return null;

        return Directory.EnumerateFiles(_directory, FileName, SearchOption.AllDirectories)
            .Where(file => !string.Equals(System.IO.Path.GetFullPath(file), System.IO.Path.GetFullPath(FilePath),
                StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();
    }

    private Settings Read(string file)
    {
        try
        {
            var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(file), JsonOptions) ?? new Settings();
            settings.Mountables ??= new List<Mountable>();
            return settings;
        }
        catch (JsonException)
        {
            // Keep the broken file around instead of silently overwriting it on the next save.
            File.Copy(file, System.IO.Path.ChangeExtension(file, ".invalid.json"), true);
            return new Settings();
        }
    }
}
