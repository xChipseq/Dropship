using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;

namespace Dropship;

public static class DepotDownloader
{
    public static bool Embedded { get; private set; } = false;
    public static string ExePath { get; private set; }

    public static bool RememberLoginData = false;
    public static string LoginUsername = null;
    public static string LoginPassword = null;

    public static void Load()
    {
        try
        {
            Embedded = ExtractFromResources();
            if (Embedded) // resource is avaliable and it's extracted into the temp folder
            {
                ExePath = Path.Combine(Path.GetTempPath(), "DropshipDepotDownloader.exe");
            }
            else // resource is not embedded
            {
                ExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DepotDownloader.exe");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"an error occurred while loading depotdownloader: {ex.Message}");
        }
    }

    public static bool ExtractFromResources()
    {
        using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dropship.DepotDownloader.DepotDownloader.exe"))
        {
            if (resourceStream == null)
            {
                Logger.Warn("DepotDownloader.exe is not embedded");
                return false;
            }

            string tempPath = Path.Combine(Path.GetTempPath(), "DropshipDepotDownloader.exe");

            // Write the resource to the temp file
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                resourceStream.CopyTo(fileStream);
            }
            Logger.Warn($"DepotDownloader.exe has been extracted to {tempPath}");
        }
        return true;
    }

    public static bool DownloadBuild(string buildid, string versionName)
    {
        string buildPath = Path.Combine(Directories.VersionsFolder, versionName);
        if (Directory.Exists(versionName)) Directory.Delete(versionName);
        Directory.CreateDirectory(buildPath);
        Logger.Warn($"versions path - {Directories.VersionsFolder}");
        Logger.Warn($"build path - {buildPath}");
        string arguments = $"-app 945360 -depot 945361 -manifest {buildid} -username {LoginUsername} -password {LoginPassword} -dir \"{buildPath}\"";
        if (RememberLoginData) arguments += " -remember-password";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ExePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            }
        };

        try
        {
            process.Start();

            bool downloadingStarted = false;
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.StartsWith("Pre") || line.StartsWith("Validating"))
                    downloadingStarted = true;

                if (!downloadingStarted)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[DepotDownloader] {line}"); // Output DepotDownloader logs
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (line[6] == '%')
                {
                    double percentage = double.Parse(line[..7].TrimEnd('%')) / 100.0;

                    int progressBarWidth = 30;
                    int filledBars = (int)(percentage * progressBarWidth);
                    int emptyBars = progressBarWidth - filledBars;

                    // Build the progress bar
                    string progressBar = new string('#', filledBars) + new string('-', emptyBars);

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"[{progressBar}] {percentage * 100:0.00}%");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            Console.WriteLine();

            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Logger.Error($"DepotDownloader Error: {ex.Message}\n{ex.StackTrace}");
            process.Kill();
            return false;
        }

        return true;
    }

    public static List<string> GetInstalledVersions()
    {
        List<string> versions = new();
        foreach (var version in Directory.GetDirectories(Directories.VersionsFolder))
        {
            versions.Add(version.Split("\\").Last());
        }

        return versions;
    }

    public static bool IsVersionInstalled(string version)
    {
        var versions = GetInstalledVersions();
        return versions.Contains(version);
    }
}