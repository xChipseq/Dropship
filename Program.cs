using System;
using System.Diagnostics;
using Dropship.DepotDownloader;
using Microsoft.Win32;

namespace Dropship;

public class Program
{
    public static Stopwatch RuntimeTimer { get; private set; }
    static void Main(string[] args)
    {
        RuntimeTimer = new Stopwatch();
        RuntimeTimer.Start();
        
        Directories.Load();
        DepotDownloaderLoader.Load();
        DepotDownloaderLoader.DecryptLogin();
        CommandManager.RegisterCommands();

        Logger.Title();
        
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            if (input.ToLower() == "close") break;

            CommandManager.ExecuteCommand(input);
        }

        // things i need in this app:
        // 1. among us depots downloading - use depotdownloader, ask for account info, download the depots in a folder near the launcher
        // 2. most popular mods downloading - have all links and versions as a github repo json and also download them in seperate files
        // 3. launching games directly from the launcher with steam .exe file, bepinex argument stuff etc.


        /*
        // Paths
        string steamDir = GetSteamDirectory();
        if (steamDir == "")
        {
            Console.ReadLine();
            return;
        }
        string steamExePath = @$"{steamDir}\steam.exe";
        string bepinexPreloaderPath = @"C:\Users\chips\AppData\Roaming\Thunderstore Mod Manager\DataFolder\LethalCompany\profiles\dzisiaj\BepInEx\core\BepInEx.Preloader.dll";
        string gameId = "1966720";
        string arguments = $"--doorstop-enable true --doorstop-target \"{bepinexPreloaderPath}\"";

        try
        {
            // Configure the process start info
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = steamExePath,
                Arguments = $"-console",
                UseShellExecute = false
            };

            // Start the game process
            Process gameProcess = Process.Start(processStartInfo);
            if (gameProcess != null)
            {
                Console.WriteLine($"game started successfully with PID: {gameProcess.Id}");
            }
            else
            {
                Console.WriteLine("failed to start the game process.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error starting the game: {ex.Message}");
        }

        Console.ReadLine();
        */
    }

    static string GetSteamDirectory()
    {
        Console.WriteLine("searching for steam directory...");
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
                    return installPath.ToString();
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
                    return installPath.ToString();
                }
            }
        }

        // Steam directory not found
        Console.WriteLine("steam directory not found");
        return "";
    }
}
