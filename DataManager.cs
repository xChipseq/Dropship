using System.IO.Compression;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Dropship.Objects;

namespace Dropship;

public static class DataManager
{
    public static readonly HttpClient _HttpClient = new()
    {
        DefaultRequestHeaders =
        {
            {"User-Agent", "Dropship-Mod-Manager"},
        },
    };

    public static Dictionary<string, ModData> ModList { get; private set; } = new();
    public static Dictionary<string, string> ManifestVersionsList { get; private set; } = new();

    public static string BepInExFile { get; } = "BepInEx-6.0.0-pre.2.zip";
    public static string BepInExUrl { get; } = "https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x86-6.0.0-pre.2.zip";

    public static async Task Load()
    {
        await DownloadModData(false); // not networked for now
        await DownloadManifestData(false);
        Logger.Log("All data loaded");
    }

    public static async Task DownloadModData(bool networked)
    {
        try
        {
            if (networked)
            {
                Logger.Log("Downloading modlist...");
                var request = await _HttpClient.GetAsync("https://github.com/xChipseq/Dropship-Mod-Manager/raw/main/Data/mods.json");
                var json = await request.Content.ReadFromJsonAsync<ModList>();
                ModList = json.List;
            }
            else
            {
                Logger.Log("Getting modlist from resources...");
                string json = LoadJson("Dropship.Data.mods.json");
                ModList list = JsonSerializer.Deserialize<ModList>(json);
                ModList = list.List;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load data: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static async Task DownloadManifestData(bool networked)
    {
        try
        {
            if (networked)
            {
                Logger.Log("Downloading manifest data...");
                var request = await _HttpClient.GetAsync("https://github.com/xChipseq/Dropship-Mod-Manager/raw/main/Data/game.json");
                var json = await request.Content.ReadFromJsonAsync<ManifestData>();
                ManifestVersionsList = json.List;
            }
            else
            {
                Logger.Log("Getting manifest data from resources...");
                string json = LoadJson("Dropship.Data.game.json");
                ManifestData data = JsonSerializer.Deserialize<ManifestData>(json);
                ManifestVersionsList = data.List;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load manifest data: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static Dictionary<string, List<string>> GetDownloadedMods()
    {
        string modsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

        Dictionary<string, List<string>> mods = new();

        foreach (string mod in Directory.GetDirectories(modsPath))
        {
            if (mod.EndsWith("BepInEx")) continue; // Temporary solution: skip bepinex

            List<string> versions = new();
            foreach (string version in Directory.GetFiles(mod))
            {
                string versionName = version.Split("\\").Last().Split("..")[1];
                string noExtension = versionName.Substring(0, versionName.LastIndexOf('.'));
                versions.Add(noExtension);
            }
            string modName = mod.Split("\\").Last().Split("..")[0];
            if (versions.Count != 0) mods.Add(modName, versions);
        }

        return mods;
    }

    public static bool IsModDownloaded(string modName, string modVersion = null)
    {
        if (modVersion != null) // check for specific version
        {
            var mods = GetDownloadedMods();
            foreach (var mod in mods)
            {
                if (mod.Key == modName && mod.Value.Contains(modVersion))
                {
                    return true;
                }
            }
        }
        else // check for any version
        {
            var mods = GetDownloadedMods();
            foreach (var mod in mods)
            {
                if (mod.Key == modName)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static async Task<bool> DownloadMod(string modName, string version, bool dependency = false)
    {
        ModData mod = ModList[modName];
        string modFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods", $"{modName}");
        try
        {
            Release releaseToDownload = null;
            if (version == "latest")
            {
                string url = ModList[modName].ReleasesUrl + "/latest"; // link to the latest release
                var latestresponse = _HttpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).Result;
                releaseToDownload = await latestresponse.Content.ReadFromJsonAsync<Release>();
            }
            else
            {
                // search for the tag with specified version
                var searchresponse = _HttpClient.GetAsync(ModList[modName].ReleasesUrl, HttpCompletionOption.ResponseHeadersRead).Result;
                List<Release> releases = await searchresponse.Content.ReadFromJsonAsync<List<Release>>();

                foreach (var release in releases)
                {
                    if (release.Version == version)
                    {
                        releaseToDownload = release;
                        break;
                    }
                }

                if (releaseToDownload == null)
                {
                    Console.WriteLine($"No mod with version \"{version}\" found");
                    return false;
                }
            }

            ReleaseAsset assetToDownload = null;
            foreach (var asset in releaseToDownload.Assets)
            {
                if (asset.FileName.EndsWith(".dll"))
                {
                    assetToDownload = asset;
                    break;
                }
            }
            if (Path.Exists(Path.Combine(modFolderPath, $"{modName}..{releaseToDownload.Version}.dll")))
            {
                Console.WriteLine("Mod with this version is already installed");
                return false;
            }
            if (assetToDownload == null)
            {
                Console.WriteLine($"The release you are trying to download has invalid assets");
                return false;
            }

            // Create mod folder
            if (!Directory.Exists(modFolderPath))
                Directory.CreateDirectory(modFolderPath);

            using HttpResponseMessage response = await _HttpClient.GetAsync(assetToDownload.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using Stream contentStream = await response.Content.ReadAsStreamAsync();
            await using FileStream fileStream = new FileStream(Path.Combine(modFolderPath, $"{modName}..{releaseToDownload.Version}.dll"), FileMode.Create, FileAccess.Write, FileShare.None);

            long? totalBytes = response.Content.Headers.ContentLength;
            byte[] buffer = new byte[8192]; // 8KB buffer size
            long totalRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalRead += bytesRead;

                double percentage = double.Parse($"{totalRead * 100 / totalBytes:0.00}") / 100.0;
                int filledBars = (int)(percentage * 30);
                int emptyBars = 30 - filledBars;
                string progressBar = new string('#', filledBars) + new string('-', emptyBars);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"{modName}: [{progressBar}] {percentage * 100:0.00}%");
                Console.ForegroundColor = ConsoleColor.White;
            }
            contentStream.Close();

            // Check for dependencies
            if (ModList[modName].Dependencies.Count > 0 && !dependency)
            {
                Console.WriteLine("\nChecking for dependencies...");

                foreach (var d in ModList[modName].Dependencies)
                {
                    if (!IsModDownloaded(d.Value, d.Key))
                    {
                        await DownloadMod(d.Key, d.Value, true);
                    }
                }

                Console.WriteLine("\nAll dependencies installed");
            }

            if (!dependency)
            {
                Console.WriteLine($"{modName} successfully installed");
                if (ModList[modName].Versioning.ContainsKey(releaseToDownload.Version))
                {
                    if (DepotDownloader.IsVersionInstalled(releaseToDownload.Version))
                    {
                        Console.WriteLine($"Among Us {ModList[modName].Versioning[releaseToDownload.Version]} is specified for {modName} {releaseToDownload.Version}\nDownload it? [y/n]");
                        string input = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            if (input.ToLower() == "y")
                            {
                                string buildid = ManifestVersionsList[ModList[modName].Versioning[releaseToDownload.Version]];
                                DepotDownloader.DownloadBuild(buildid, ModList[modName].Versioning[releaseToDownload.Version]);
                            }
                        }
                    }
                }
                Console.Write("\n> ");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to download mod {modName} ({version}): {ex.Message}\n{ex.StackTrace}");
            return false;
        }

        return true;
    }

    public static async Task DownloadBepInEx()
    {
        string bepinexFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods", "BepInEx");
        try
        {
            if (File.Exists(Path.Combine(bepinexFolderPath, BepInExFile)))
            {
                Console.WriteLine($"BepInEx is already installed");
                return;
            }

            // Create mod folder
            if (!Directory.Exists(bepinexFolderPath))
                Directory.CreateDirectory(bepinexFolderPath);

            using HttpResponseMessage response = await _HttpClient.GetAsync(BepInExUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using Stream contentStream = await response.Content.ReadAsStreamAsync();
            await using FileStream fileStream = new FileStream(Path.Combine(bepinexFolderPath, BepInExFile), FileMode.Create, FileAccess.Write, FileShare.None);

            long? totalBytes = response.Content.Headers.ContentLength;
            byte[] buffer = new byte[8192]; // 8KB buffer size
            long totalRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalRead += bytesRead;

                double percentage = double.Parse($"{totalRead * 100 / totalBytes:0.00}") / 100.0;
                int filledBars = (int)(percentage * 30);
                int emptyBars = 30 - filledBars;
                string progressBar = new string('#', filledBars) + new string('-', emptyBars);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"BepInEx: [{progressBar}] {percentage * 100:0.00}%");
            }
            contentStream.Close();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\nBepInEx 6.0.0-pre.2 successfully installed");

        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to download BepInEx: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static async Task UnzipFileAsync(string zipFilePath, string extractPath)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue;
                string entryPath = Path.Combine(extractPath, entry.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
                entry.ExtractToFile(entryPath, overwrite: true);
            }
        }
    }

    public static string LoadJson(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (var stream = assembly.GetManifestResourceStream(path))
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            string jsonString = reader.ReadToEnd();
            return jsonString;
        }
    }
}