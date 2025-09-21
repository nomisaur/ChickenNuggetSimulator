using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChickenNuggetSimulator.Core;

public sealed class SaveData
{
    public int Nuggets { get; set; } = 0;
}

public class SaveSystem
{
    public CNS game;

    public SaveData data;

    public SaveSystem(CNS game)
    {
        this.game = game;
        data = Load();
    }

    private const string Company = "Woof";
    private const string Game = "ChickenNuggetSimulator";
    private const string FileName = "save.json";

    // One JsonSerializerOptions instance to keep things consistent
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// Cross-platform per-user app data directory (inside the app sandbox on mobile).
    /// </summary>
    public static string GetSaveDirectory()
    {
        // Desktop (Win/macOS/Linux) → LocalApplicationData is correct.
        // iOS → we prefer Library/Application Support (Apple’s guideline).
        // Android → SpecialFolder.Personal maps to /data/data/<pkg>/files (internal storage).
        string baseDir;

        if (OperatingSystem.IsIOS())
        {
            // Documents/.. /Library/Application Support
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var libAppSupport = Path.GetFullPath(
                Path.Combine(docs, "..", "Library", "Application Support")
            );
            baseDir = libAppSupport;
        }
        else if (OperatingSystem.IsAndroid())
        {
            // Personal is the app's internal files dir.
            baseDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
        else
        {
            // Windows/macOS/Linux
            baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        return Path.Combine(baseDir, Company, Game);
    }

    public static string GetSavePath() => Path.Combine(GetSaveDirectory(), FileName);

    private static string GetBackupPath() => Path.Combine(GetSaveDirectory(), FileName + ".bak");

    /// <summary>
    /// Save atomically: write to temp, then replace. Also maintains a .bak.
    /// </summary>
    public void Save()
    {
        Directory.CreateDirectory(GetSaveDirectory());

        var path = GetSavePath();
        var tmp = path + ".tmp";
        var bak = GetBackupPath();

        var json = JsonSerializer.Serialize(data, JsonOpts);

        // 1) Write temp
        File.WriteAllText(tmp, json);

        // 2) Move current to .bak (if exists)
        if (File.Exists(path))
        {
            // Replace .bak with current safely
            File.Copy(path, bak, overwrite: true);
        }

        // 3) Replace current with temp (atomic on most platforms)
        if (File.Exists(path))
            File.Replace(tmp, path, bak);
        else
            File.Move(tmp, path);
    }

    /// <summary>
    /// Load, with fallback to .bak if main is corrupt.
    /// </summary>
    public static SaveData? Load()
    {
        var path = GetSavePath();
        var bak = GetBackupPath();

        // Try main
        var data = TryLoadFile(path);
        if (data != null)
            return data;

        // Fallback to backup
        data = TryLoadFile(bak);
        if (data != null)
        {
            // Restore backup as current to heal future loads
            File.Copy(bak, path, overwrite: true);
            return data;
        }

        // No save found
        return new SaveData();
    }

    private static SaveData? TryLoadFile(string path)
    {
        try
        {
            if (!File.Exists(path))
                return null;
            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<SaveData>(json, JsonOpts);

            if (data == null)
                return null;

            return data;
        }
        catch
        {
            // corrupted or unreadable
            return null;
        }
    }

    /// <summary>
    /// Optional: delete save (e.g., for debugging or a “New Game” option).
    /// </summary>
    public static void Delete()
    {
        try
        {
            if (File.Exists(GetSavePath()))
                File.Delete(GetSavePath());
        }
        catch { }
        try
        {
            if (File.Exists(GetBackupPath()))
                File.Delete(GetBackupPath());
        }
        catch { }
    }
}
