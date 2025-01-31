using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Dropship.Objects;

namespace Dropship;

public static class ProfileManager
{
    public static Dictionary<string, ProfileData> Profiles = new();

    public static string EditProfile = null;

    public static void LoadProfiles()
    {
        Logger.Log("Loading profiles...");
        Profiles.Clear();
        try
        {
            foreach (var path in Directory.GetDirectories(Directories.ProfilesFolder))
            {
                string jsonPath = Path.Combine(path, "profile.json");
                string json = File.ReadAllText(jsonPath);
                ProfileData data = JsonSerializer.Deserialize<ProfileData>(json);

                Profiles.Add(data.Name, data);
                Logger.Log($"Loaded \"{data.Name}\" profile");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error while loading profiles: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void SaveProfiles()
    {
        foreach (var profile in Profiles)
        {
            File.WriteAllText(Path.Combine(profile.Value.Path, "profile.json"), JsonSerializer.Serialize(profile.Value, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }

    public static void CreateProfile(string profileName, string amongUsVersion)
    {
        try
        {
            string profilesPath = Directories.ProfilesFolder;
            string bepinexFolderPath = Path.Combine(Directories.ModsFolder, "BepInEx");
            string profileFolder = Path.Combine(profilesPath, profileName);

            if (Directory.Exists(profileFolder))
            {
                Console.WriteLine($"Profile with name \"{profileName}\" already exists, choose a different name");
                return;
            }
            if (amongUsVersion != "steam")
            {
                bool validVersion = Regex.IsMatch(amongUsVersion, @"^\d{4}\.\d{1,2}\.\d{1,2}$");
                if (!validVersion)
                {
                    Console.WriteLine($"The selected version \"{amongUsVersion}\" is not in a correct format");
                    return;
                }

                if (!DepotDownloader.IsVersionInstalled(amongUsVersion))
                {
                    Console.WriteLine($"The version {amongUsVersion} is not installed\nWould you like to download it or use the steam installation? [y/steam]");
                    string input = Console.ReadLine() ?? "";
                    while (string.IsNullOrWhiteSpace(input) && (input != "y" || input != "steam"))
                    {
                        Console.WriteLine($"Would you like to download it or use the steam installation? [y/steam]");
                        input = Console.ReadLine();
                    }

                    if (input == "steam")
                    {
                        Console.WriteLine("Profile will use the steam installation");
                        amongUsVersion = "steam";
                    }
                    else if (input == "y")
                    {
                        if (DataManager.ManifestVersionsList.ContainsKey(amongUsVersion))
                        {
                            DepotDownloader.DownloadBuild(DataManager.ManifestVersionsList[amongUsVersion], amongUsVersion);
                        }
                        else
                        {
                            Console.WriteLine($"Version not found: {amongUsVersion}\nWe might not have the version you want. Please use download_custom_among command");
                            return;
                        }
                    }
                }

                if (amongUsVersion != "steam")
                    Console.WriteLine($"Profile will use Among Us {amongUsVersion}");
            }
            else // Uses the steam version
            {
                Console.WriteLine("Profile will use the steam installation");
            }

            // downloading BepInEx if it's not there
            if (!File.Exists(Path.Combine(bepinexFolderPath, DataManager.BepInExFile)))
            {
                DataManager.DownloadBepInEx().GetAwaiter().GetResult();
            }

            Directory.CreateDirectory(profileFolder);
            DataManager.UnzipFileAsync(Path.Combine(bepinexFolderPath, DataManager.BepInExFile), profileFolder);

            ProfileData data = new();
            data.Name = profileName;
            data.Path = profileFolder;
            data.AmongUsVersion = amongUsVersion;
            data.Mods = new();
            File.WriteAllText(Path.Combine(profileFolder, "profile.json"), JsonSerializer.Serialize(data));
            LoadProfiles();
        }
        catch (Exception ex)
        {
            Logger.Error($"Error while creating {profileName} profile: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void DeleteProfile(string profileName)
    {
        string profileFolder = Path.Combine(Directories.ProfilesFolder, profileName);

        if (!Directory.Exists(profileName))
        {
            Console.WriteLine($"Profile with name \"{profileName}\" does not exist");
            return;
        }

        Console.WriteLine($"Are you sure you want to PERMANENTLY delete {profileName}? [y/n]");
        string input = Console.ReadLine();
        if (input == "y")
        {
            Directory.Delete(profileFolder);
            return;
        }

        Console.WriteLine("Operation canceled");
    }

    public static void AddMod(string modName, string version)
    {
        if (EditProfile == null)
        {
            Console.WriteLine("You are not editing any profile\nUse edit_profile command");
            return;
        }

        // check if mod exists
        if (!DataManager.ModList.ContainsKey(modName))
        {
            Console.WriteLine($"The mod \"{modName}\" does not exist");
            return;
        }

        // check if the mod is already added to this instance
        if (Profiles[EditProfile].Mods.ContainsKey(modName))
        {
            Console.WriteLine("Mod has been already added to this profile");
            return;
        }

        // check the mod and profile version
        string modAmongVersion = "none";
        if (DataManager.ModList[modName].Versioning.ContainsKey(version)) modAmongVersion = DataManager.ModList[modName].Versioning[version];
        string profileAmongVersion = Profiles[EditProfile].AmongUsVersion;
        if (modAmongVersion != profileAmongVersion && modAmongVersion != "none")
        {
            Console.WriteLine($"The mod you are trying to add has Among Us {modAmongVersion} specified, but the {EditProfile} is on {profileAmongVersion}\nDo you wish to procced? [y/n]");
            string input = Console.ReadLine();
            if (input == "n")
            {
                return;
            }
        }

        // check if mod is installed, if not - download it 
        if (!DataManager.IsModDownloaded(modName, version))
        {
            bool success = DataManager.DownloadMod(modName, version).GetAwaiter().GetResult();
            if (!success) return;
            Task.Delay(2000).GetAwaiter().GetResult(); // just in case
        }

        // create the plugins folder if it's not there (probably isn't)
        if (!Directory.Exists(Path.Combine(Profiles[EditProfile].Path, "BepInEx\\plugins")))
        {
            Directory.CreateDirectory(Path.Combine(Profiles[EditProfile].Path, "BepInEx\\plugins"));
        }

        // copy the dll
        string dllPath = Path.Combine(Directories.ModsFolder, modName, $"{modName}..{version}.dll");
        string destPath = Path.Combine(Profiles[EditProfile].Path, "BepInEx\\plugins", $"{modName}..{version}.dll");
        File.Copy(dllPath, destPath);
        Profiles[EditProfile].Mods.Add(modName, version);

        // copy all dependencies if any
        foreach (var dependency in DataManager.ModList[modName].Dependencies)
        {
            if (Profiles[EditProfile].Mods.ContainsKey(dependency.Key)) continue;

            string dependecnyDllPath = Path.Combine(Directories.ModsFolder, dependency.Key, $"{dependency.Key}..{dependency.Value}.dll");
            string dependencyDestPath = Path.Combine(Profiles[EditProfile].Path, "BepInEx\\plugins", $"{dependency.Key}..{dependency.Value}.dll");
            File.Copy(dependecnyDllPath, dependencyDestPath);

            Profiles[EditProfile].Mods.Add(dependency.Key, dependency.Value);
        }

        SaveProfiles();
        LoadProfiles();

        Console.WriteLine($"The mod {modName} ({version}) and all it's dependencies have been added to {EditProfile} profile");
    }

    public static void RemoveMod(string modName)
    {
        if (EditProfile == null)
        {
            Console.WriteLine("You are not editing any profile\nUse edit_profile command");
            return;
        }

        // check if mod exists
        if (!DataManager.ModList.ContainsKey(modName))
        {
            Console.WriteLine($"The mod \"{modName}\" does not exist");
        }

        // check if the mod is added to this profile
        if (!Profiles[EditProfile].Mods.ContainsKey(modName))
        {
            Console.WriteLine("Profile does not contain this mod");
            return;
        }

        // copy the dll
        string modPath = Path.Combine(Profiles[EditProfile].Path, "BepInEx\\plugins", $"{modName}..{Profiles[EditProfile].Mods[modName]}.dll");
        if (File.Exists(modPath))
        {
            File.Delete(modPath);
        }
        Profiles[EditProfile].Mods.Remove(modName);

        SaveProfiles();
        LoadProfiles();

        Console.WriteLine($"The mod {modName} has been removed from {EditProfile} profile");
    }

    public static void LaunchProfile(string profile)
    {
        if (!Profiles.ContainsKey(profile))
        {
            Console.WriteLine($"Profile \"{profile}\" not found");
            return;
        }
        ProfileData profileData = Profiles[profile];

        // Argument stuff
        string bepinexPath = Path.Combine(profileData.Path, "BepInEx\\core\\BepInEx.Unity.IL2CPP.dll");
        string coreclrDir = Path.Combine(profileData.Path, "dotnet");
        string coreclrPath = Path.Combine(profileData.Path, "dotnet\\coreclr.dll");
        string[] arguments = [
            "--doorstop-enable true",
            $"--doorstop-target-assembly \"{bepinexPath}\"",
            $"--doorstop-clr-corlib-dir \"{coreclrDir}\"",
            $"--doorstop-clr-runtime-coreclr-path \"{coreclrPath}\""
        ];

        // Steam
        string steamPath = Program.GetSteamExeFile();
        string steamArguments = $"-applaunch 945360 {string.Join(" ", arguments)}";
        bool useSteam = profileData.AmongUsVersion == "steam";
        bool steamError = steamPath is null;

        string amongPath = "";
        if (useSteam && steamError)
        {
            amongPath = AmongUsLocator.FindAmongUs();
        }
        else if (!useSteam)
        {
            amongPath = Path.Combine(Directories.VersionsFolder, profileData.AmongUsVersion);
            Logger.Warn(amongPath);
            if (!Directory.Exists(amongPath))
            {
                Console.WriteLine($"The selected version {profileData.AmongUsVersion} for {profile} profile is not present\nDownload it or change it to steam with change_version command");
                return;
            }
        }

        string amongExecutable = Path.Combine(amongPath, "Among Us.exe");

        // Add doorstop to the game files
        if (!File.Exists(Path.Combine(amongPath, "winhttp.dll")))
            File.Copy(Path.Combine(profileData.Path, "winhttp.dll"), Path.Combine(amongPath, "winhttp.dll")); // doorstop dll
        if (!File.Exists(Path.Combine(amongPath, "doorstop_config.ini")))
            File.Copy(Path.Combine(profileData.Path, "doorstop_config.ini"), Path.Combine(amongPath, "doorstop_config.ini")); // doorstop config
        File.WriteAllText(Path.Combine(amongPath, "steam_appid.txt"), "945360"); // steam_appid.txt, just in case

        // Launch Among Us on steam with the arguments
        var gameProcess = Process.Start(new ProcessStartInfo()
        {
            FileName = useSteam ? steamPath : amongExecutable, // launch the .exe file directly if steam was not found. this should work because we created a steam_appid.txt file
            Arguments = useSteam ? steamArguments : string.Join(" ", arguments),
        });

        if (useSteam)
        {
            Console.WriteLine($"Profile {profile} launched via Steam!");
        }
        else
        {
            Console.WriteLine($"Profile {profile} launched!");
        }
    }
}