using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;

namespace Dropship.DepotDownloader;

public static class DepotDownloaderLoader
{
    public static bool Loaded { get; private set; } = false;
    public static string ExePath { get; private set; }

    public static bool RememberLoginData = false;
    public static string LoginUsername = null;
    public static string LoginPassword = null;

    public static void Load()
    {
        try
        {
            string resourceName = "Dropship.DepotDownloader.DepotDownloader.exe";

            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    Logger.Warn("depot downloader resource not found");
                    return;
                }

                string tempPath = Path.Combine(Path.GetTempPath(), "DropshipDepotDownloader.exe");

                using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }

                Loaded = true;
                ExePath = tempPath;
                Logger.Log($"depot downloader has been successfully loaded - {tempPath}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"an error occurred while loading depotdownloader: {ex.Message}");
        }
    }

    public static bool DownloadBuild(string buildid)
    {
        if (Loaded == false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("DepotDownloader failed to load. Restart the program. This also might be a permissions issue");
            Console.ForegroundColor = ConsoleColor.White;
            return false;
        }
        string buildPath = Path.Combine(Directories.VersionsFolder, buildid);
        if (Directory.Exists(buildPath)) Directory.Delete(buildPath);
        Directory.CreateDirectory(buildPath);
        string arguments = $"-app 945360 -depot 945361 -manifest {buildid} -dir {buildPath} -username {LoginUsername} -password {LoginPassword}";
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

    public static void EncryptLogin()
    {
        string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DropshipData");
        if (!Directory.Exists(appDataFolder)) Directory.CreateDirectory(appDataFolder);
        File.SetAttributes(appDataFolder, FileAttributes.Hidden);

        string infoPath = Path.Combine(appDataFolder, "drop.ship");
        var random = new Random();
        List<string> info = new()
        {
            Convert.ToBase64String(Encoding.UTF8.GetBytes(random.Next(1, 99999).ToString())),
            Convert.ToBase64String(Encoding.UTF8.GetBytes(random.Next(1, 99999).ToString())),
        };
        File.WriteAllText(infoPath, Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(info))));

        string ciphertextPath = Path.Combine(appDataFolder, $"{info[0]}.bin");
        string entropyPath = Path.Combine(appDataFolder, $"{info[1]}.bin");

        string plainText = $"{LoginUsername}::{LoginPassword}";
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
        plainText = "";

        // Generate entropy
        byte[] entropy = new byte[20];
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(entropy);
        }

        // Encrypt using ProtectedData
        byte[] ciphertext = ProtectedData.Protect(plaintextBytes, entropy, DataProtectionScope.CurrentUser);

        // Save ciphertext and entropy
        File.WriteAllBytes(ciphertextPath, ciphertext);
        File.WriteAllBytes(entropyPath, entropy);

        Logger.Log($"Login data encrypted and saved");
    }

    public static void DecryptLogin()
    {
        string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DropshipData");
        string infoPath = Path.Combine(appDataFolder, "drop.ship");
        if (!File.Exists(infoPath))
        {
            Logger.Warn("No login data to load");
            return;
        }
        Logger.Log("Loading login data..");

        string infoJson = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(infoPath)));
        List<string> info = JsonSerializer.Deserialize<List<string>>(infoJson);

        byte[] ciphertext = File.ReadAllBytes(Path.Combine(appDataFolder, $"{info[0]}.bin"));
        byte[] entropy = File.ReadAllBytes(Path.Combine(appDataFolder, $"{info[1]}.bin"));

        byte[] plaintextBytes = ProtectedData.Unprotect(ciphertext, entropy, DataProtectionScope.CurrentUser);

        string login = Encoding.UTF8.GetString(plaintextBytes);
        var elements = login.Split("::");
        LoginUsername = elements[0];
        LoginPassword = elements[1];
    }
}