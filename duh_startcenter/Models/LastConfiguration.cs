using System.Text.Json.Serialization;

namespace NXStartCenter;

public sealed class LastConfiguration
{
    [JsonPropertyName("last_customer")]
    public string? LastCustomer { get; set; }

    [JsonPropertyName("last_version")]
    public string? LastVersion { get; set; }

    [JsonPropertyName("last_machine")]
    public string? LastMachine { get; set; }

    [JsonPropertyName("last_native_version")]
    public string? LastNativeVersion { get; set; }

    [JsonPropertyName("last_postbuilder_version")]
    public string? LastPostbuilderVersion { get; set; }

    [JsonPropertyName("last_language")]
    public string LastLanguage { get; set; } = "german";

    [JsonPropertyName("last_load_pp")]
    public bool LastLoadPp { get; set; }

    [JsonPropertyName("last_load_installed_machines")]
    public bool LastLoadInstalledMachines { get; set; }

    [JsonPropertyName("last_load_tool")]
    public bool LastLoadTool { get; set; }

    [JsonPropertyName("last_load_device")]
    public bool LastLoadDevice { get; set; }

    [JsonPropertyName("last_load_feed")]
    public bool LastLoadFeed { get; set; }

    [JsonPropertyName("last_start_with_tc")]
    public bool LastTcCheck { get; set; }

    [JsonPropertyName("last_used_cloud_license")]
    public bool LastCloudLicenseCheck { get; set; }

    [JsonPropertyName("last_load_full_resource_folder")]
    public bool LastLoadFullResourceDir { get; set; }
}
