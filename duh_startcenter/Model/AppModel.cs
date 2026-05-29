using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

using NXStartCenter;
namespace NXStartCenter.Model;

public sealed class AppModel
{

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public string ConfigPath { get; }
    public AppConfig Config { get; private set; }
    public AppSettings Settings => Config.Settings;

    public AppInfo AppInfo { get; private set; } = new();
    public LastConfiguration Last => Config.LastConfiguration;

    public string SelectedCustomer { get; set; } = string.Empty;
    public string SelectedVersion { get; set; } = string.Empty;
    public string SelectedMachine { get; set; } = string.Empty;
    public string SelectedNativeVersion { get; set; } = string.Empty;
    public string SelectedPostbuilderVersion { get; set; } = string.Empty;
    public bool StartNxWithCloudLicense { get; set; }
    public bool StartNxManaged { get; set; } 

    public IReadOnlyList<string> Customers { get; private set; } = [];
    public IReadOnlyList<string> Versions { get; private set; } = [];
    public IReadOnlyList<string> Machines { get; private set; } = [];
    public IReadOnlyList<string> NativeVersions { get; private set; } = [];
    public IReadOnlyList<string> PostbuilderVersions { get; private set; } = [];

    public string ForkPath { get; set; } = "";

    public string[] MachineControllers { get; } = ["Sinumerik", "Fanuk", "Okuma", "HeidenhainTNC"];
    public string EnvFolderName { get; } = "5_Umgebung";

    public Dictionary<string, string> MachineTypes { get; } = new()
    {
        ["Mill machine"] = "MDM0101",
        ["TurnMill machine"] = "MDM0104",
        ["Lathe machine"] = "MDM0201",
        ["MillTurn machine"] = "MDM0204",
        ["Wedm machine"] = "MDM0301",
        ["Robot machine"] = "MDM0401",
        ["Generic machine"] = "MDM0901",
    };

    private AppModel(string configPath, AppConfig config)
    {
        ConfigPath = configPath;
        Config = config;
        RefreshAll();
    }

    public static AppModel Load(string configPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
        if (!File.Exists(configPath))
        {
            var empty = new AppConfig();
            File.WriteAllText(configPath, JsonSerializer.Serialize(empty, JsonOptions));
            return new AppModel(configPath, empty);
        }

        var json = File.ReadAllText(configPath);
        var cfg = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        cfg.Settings ??= new AppSettings();
        cfg.LastConfiguration ??= new LastConfiguration();
        return new AppModel(configPath, cfg);
    }

    public void Save() => File.WriteAllText(ConfigPath, JsonSerializer.Serialize(Config, JsonOptions));

    public void RefreshAll()
    {
        NativeVersions = FindNxVersions(Settings.NxInstallationPath);
        SelectedNativeVersion = Pick(Last.LastNativeVersion, NativeVersions);
        PostbuilderVersions = NativeVersions;
        SelectedPostbuilderVersion = Pick(Last.LastPostbuilderVersion, PostbuilderVersions);
        Customers = DirectoryNames(Settings.CustomerEnvironmentPath);
        SelectedCustomer = Pick(Last.LastCustomer, Customers);
        StartNxManaged = Last.LastTcCheck;
        StartNxWithCloudLicense = Last.LastCloudLicenseCheck;

        RefreshVersions();
    }

    public void RefreshVersions()
    {
        Versions = DirectoryNames(Path.Combine(Settings.CustomerEnvironmentPath, SelectedCustomer, EnvFolderName))
            .Where(x => x.StartsWith("NX", StringComparison.OrdinalIgnoreCase)).Reverse().ToArray();
        SelectedVersion = Pick(Last.LastVersion, Versions);
        RefreshMachines();
    }

    public void RefreshMachines()
    {
        Machines = DirectoryNames(GetInstalledMachinesPath(SelectedCustomer, SelectedVersion));
        SelectedMachine = Pick(Last.LastMachine, Machines);
    }

    public string GetInstalledMachinesPath(string customername, string nxVersion)
    {
        var basePath = Path.Combine(Settings.CustomerEnvironmentPath, customername, EnvFolderName, nxVersion, "MACH", "resource", "library", "machine", "installed_machines");
        var camPath = basePath + "_" + customername;
        return Directory.Exists(camPath) ? camPath : basePath;
    }

    private static string Pick(string? last, IReadOnlyList<string> list) =>
        !string.IsNullOrWhiteSpace(last) && list.Contains(last) ? last : list.FirstOrDefault() ?? string.Empty;

    private static IReadOnlyList<string> FindNxVersions(string basePath) => DirectoryNames(basePath)
        .Where(x => x.StartsWith("NX", StringComparison.OrdinalIgnoreCase)).Reverse().ToArray();

    private static IReadOnlyList<string> DirectoryNames(string path)
    {
        try
        {
            return Directory.Exists(path) ? Directory.GetDirectories(path).Select(Path.GetFileName).Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>().ToArray() : [];
        }
        catch { return []; }
    }
}
