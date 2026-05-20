using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NXStartCenter;

public sealed class ConfigService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public string ConfigPath { get; }

    public AppConfig Config { get; private set; } = new();

    public ConfigService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = Path.Combine(appData, "NXStartCenter");

        Directory.CreateDirectory(dir);

        ConfigPath = Path.Combine(dir, "config.json");

        Load();
    }

    public void Load()
    {
        if (!File.Exists(ConfigPath))
        {
            Save();
            return;
        }

        var json = File.ReadAllText(ConfigPath);

        Config = JsonSerializer.Deserialize<AppConfig>(json, _jsonOptions) ?? new AppConfig();
        Config.Settings ??= new AppSettings();
        Config.LastConfiguration ??= new LastConfiguration();
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Config, _jsonOptions);
        File.WriteAllText(ConfigPath, json);
    }

    public void SaveLastSelection(string customer, string version, string machine)
    {
        Config.LastConfiguration.LastCustomer = customer;
        Config.LastConfiguration.LastVersion = version;
        Config.LastConfiguration.LastMachine = machine;

        Save();
    }
}