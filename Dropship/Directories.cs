namespace Dropship;

public static class Directories
{
    public static string LogPath { get; private set; }

    public static string DropshipFolder { get; private set; }
    public static string ProfilesFolder { get; private set; }
    public static string VersionsFolder { get; private set; }
    public static string ModsFolder { get; private set; }

    public static void Load()
    {
        CreateDirectories();
    }

    public static bool CreateDirectories()
    {
        string programDir = AppDomain.CurrentDomain.BaseDirectory;

        DropshipFolder = Path.Combine(programDir, "dropship");
        ProfilesFolder = Path.Combine(programDir, "profiles");
        VersionsFolder = Path.Combine(programDir, "versions");
        ModsFolder = Path.Combine(programDir, "mods");

        // Logs are important because without this everything breaks so we are doing it here
        LogPath = Path.Combine(DropshipFolder, "LatestLog.txt");
        if (File.Exists(LogPath))
        {
            File.Delete(LogPath);
        }

        try
        {
            if (!Path.Exists(DropshipFolder))
            {
                Directory.CreateDirectory(DropshipFolder);
            }
            if (!Path.Exists(ProfilesFolder))
            {
                Directory.CreateDirectory(ProfilesFolder);
            }
            if (!Path.Exists(VersionsFolder))
            {
                Directory.CreateDirectory(VersionsFolder);
            }
            if (!Path.Exists(ModsFolder))
            {
                Directory.CreateDirectory(ModsFolder);
            }

            Logger.Log("created directories");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to create directories: {ex.Message}\n{ex.StackTrace}");
            return false;
        }

        return true;
    }
}