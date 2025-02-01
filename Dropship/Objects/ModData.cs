using System.Text.Json.Serialization;

namespace Dropship.Objects;

public record class ModData(
    string Name,
    string Author,
    string Releases, // This is the github api link that are we getting releases from
    Dictionary<string, string> Dependencies, // All the mod's dependecies and their versions. Empty if none
    Dictionary<string, string> Versioning, // This list contains some predefined game versions for tags
    string? DllName = null)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DllName { get; set; } = DllName;
}

public class ModList
{
    [JsonPropertyName("Version")]
    public string Version { get; set; }

    [JsonPropertyName("List")]
    public Dictionary<string, ModData> List { get; set; }
}