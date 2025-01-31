using System.Text.Json.Serialization;

namespace Dropship.Objects;

public class ModData
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Author")]
    public string Author { get; set; }

    [JsonPropertyName("Releases")]
    public string ReleasesUrl { get; set; } // This is the github api link that are we getting releases from

    [JsonPropertyName("Dependencies")]
    public Dictionary<string, string> Dependencies { get; set; } // All the mod's dependecies and their versions. Empty if none

    [JsonPropertyName("Versioning")]
    public Dictionary<string, string> Versioning { get; set; } // This list contains some predefined game versions for tags
}

public class ModList
{
    [JsonPropertyName("Version")]
    public string Version { get; set; }

    [JsonPropertyName("List")]
    public Dictionary<string, ModData> List { get; set; }
}