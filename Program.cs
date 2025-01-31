using System.Diagnostics;
using Microsoft.Win32;

namespace Dropship;

public class Program
{
    public static Stopwatch RuntimeTimer { get; private set; }
    static async Task Main(string[] args)
    {
        RuntimeTimer = new Stopwatch();
        RuntimeTimer.Start();

#if DEBUG
        Logger.Debug = true;
#endif

        Directories.Load();
        DepotDownloader.Load();
        DepotDownloader.DecryptLogin();
        await DataManager.Load();
        ProfileManager.LoadProfiles();
        CommandManager.RegisterCommands();

        Logger.Title();

        if (!DepotDownloader.Embedded)
        {
            if (!File.Exists(DepotDownloader.ExePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING! Your release has no embedded DepotDownloader.exe and it has not been found in Dropship's .exe folder.\nYou will not be able to install builds");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            if (input.ToLower() == "close") break;

            CommandManager.ExecuteCommand(input);
        }

        ProfileManager.SaveProfiles();
    }

    public static string GetSteamExeFile()
    {
        Logger.Log("searching for steam directory...");
        string steamRegistryPath = @"SOFTWARE\WOW6432Node\Valve\Steam";
        string steamKeyName = "InstallPath";

        // Open the registry key
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(steamRegistryPath))
        {
            if (key != null)
            {
                // Read the "InstallPath" value
                object installPath = key.GetValue(steamKeyName);
                if (installPath != null)
                {
                    return installPath.ToString() + "\\steam.exe";
                }
            }
        }

        // Check the 32-bit registry key as a fallback
        steamRegistryPath = @"SOFTWARE\Valve\Steam";
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(steamRegistryPath))
        {
            if (key != null)
            {
                object installPath = key.GetValue(steamKeyName);
                if (installPath != null)
                {
                    return installPath.ToString() + "\\steam.exe";
                }
            }
        }

        // Steam directory not found
        Logger.Error("steam directory not found");
        return null;
    }
}
