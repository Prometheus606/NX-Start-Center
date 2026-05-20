using System.Text.Json.Serialization;

namespace NXStartCenter;


public sealed class AppSettings
{
    [JsonPropertyName("nx_installation_path")]
    public string NxInstallationPath { get; set; } = @"C:\Siemens\NX_Versionen";

    [JsonPropertyName("customer_environment_path")]
    public string CustomerEnvironmentPath { get; set; } = @"D:\Kundenumgebungen";

    [JsonPropertyName("licence_path")]
    public string LicencePath { get; set; } = @"C:\Siemens\License\License_ugslmd.lic";

    [JsonPropertyName("licence_server_path")]
    public string LicenceServerPath { get; set; } = @"C:\Siemens\License Server\lmtools.exe";

    [JsonPropertyName("fork_path")]
    public string ForkPath { get; set; } = $@"C:\Users\{Environment.UserName}\AppData\Local\Fork\current\Fork.exe";

    [JsonPropertyName("roles_path")]
    public string RolesPath { get; set; } = string.Empty;

    [JsonPropertyName("team")]
    public string Team { get; set; } = "PP";

    [JsonPropertyName("preferred_theme")]
    public string PreferredTheme { get; set; } = "darkly";

    [JsonPropertyName("editor")]
    public string Editor { get; set; } = "VSCode";

    [JsonPropertyName("startNXWithDebug")]
    public bool StartNxWithDebug { get; set; }
}
