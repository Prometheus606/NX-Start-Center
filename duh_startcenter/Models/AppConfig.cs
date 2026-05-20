using System.Text.Json.Serialization;

namespace NXStartCenter;

public sealed class AppConfig
{
    [JsonPropertyName("settings")]
    public AppSettings Settings { get; set; } = new();

    [JsonPropertyName("last_configuration")]
    public LastConfiguration LastConfiguration { get; set; } = new();

}
