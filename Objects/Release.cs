using System.Text.Json.Serialization;

namespace Dropship.Objects;

public class Release
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("tag_name")]
    public string Version { get; set; }

    [JsonPropertyName("assets")]
    public List<ReleaseAsset> Assets { get; set; }
}

public class ReleaseAsset
{
    [JsonPropertyName("name")]
    public string FileName { get; set; }

    [JsonPropertyName("browser_download_url")]
    public string DownloadUrl { get; set; }
}