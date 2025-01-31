using System.Text.Json.Serialization;

namespace Dropship.Objects;

public class ProfileData
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("AmongUsVersion")]
    public string AmongUsVersion { get; set; }

    [JsonPropertyName("Mods")]
    public Dictionary<string, string> Mods { get; set; }

    public string Path { get; set; }
}