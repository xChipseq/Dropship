namespace Dropship;

public static class Directories
{
    public static string LogPath { get; private set; }

    public static string DropshipFolder { get; private set; }
    public static string ProfilesFolder { get; private set; }
    public static string VersionsFolder { get; private set; }

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
                Logger.Warn($"{DropshipFolder} folder does not exist, creating one...");
                Directory.CreateDirectory(DropshipFolder);
            }
            if (!Path.Exists(ProfilesFolder))
            {
                Logger.Warn($"{ProfilesFolder} folder does not exist, creating one...");
                Directory.CreateDirectory(ProfilesFolder);
            }
            if (!Path.Exists(VersionsFolder))
            {
                Logger.Warn($"{VersionsFolder} folder does not exist, creating one...");
                Directory.CreateDirectory(VersionsFolder);
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