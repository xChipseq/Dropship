using System.Text.Json.Serialization;

namespace Dropship.Objects;

public class ManifestData
{
    [JsonPropertyName("Version")]
    public string Version { get; set; }

    [JsonPropertyName("List")]
    public Dictionary<string, string> List { get; set; }
}