using System.IO;
using System.Windows;

using NXStartCenter;
namespace NXStartCenter.Services;


public sealed class NxService(AppModel model)
{
    public string StartNativeNx()
    {
        CopyRoles(model.NativeVersion);
        var exe = GetUgrafPath(model.NativeVersion);
        if (!File.Exists(exe)) return "Version kann nicht gestartet werden!\n" + exe;
        ProcessService.StartFile(exe);
        model.Last.LastNativeVersion = model.NativeVersion;
        model.Save();
        return "NX Native wurde gestartet.";
    }

    public string StartCustomerNx()
    {
        var batch = Path.Combine(AppContext.BaseDirectory, "app", "start_routine.bat");
        if (!File.Exists(batch)) return "start_routine.bat wurde nicht gefunden.";
        var args = $"\"{model.Customer}\" \"{model.VersionName}\" \"{model.Last.LastLanguage}\" \"{model.Settings.CustomerEnvironmentPath}\" \"{model.Settings.NxInstallationPath}\" \"{Bool(model.Last.LastLoadPp)}\" \"{Bool(model.Last.LastLoadInstalledMachines)}\" \"{Bool(model.Last.LastLoadTool)}\" \"{Bool(model.Last.LastLoadDevice)}\" \"{Bool(model.Last.LastLoadFeed)}\"";
        ProcessService.StartFile(batch, args, AppContext.BaseDirectory);
        model.Last.LastCustomer = model.Customer;
        model.Last.LastVersion = model.VersionName;
        model.Last.LastMachine = model.Machine;
        model.Save();
        return "NX wurde für die Kundenumgebung gestartet.";
    }

    public string StartPostbuilder()
    {
        if (string.IsNullOrWhiteSpace(model.PostbuilderVersion)) return "Keine Postbuilder-Version gewählt.";
        var batch = Path.Combine(model.Settings.NxInstallationPath, model.PostbuilderVersion, "POSTBUILD", "post_builder.bat");
        if (!File.Exists(batch)) return "Postbuilder Version kann nicht gestartet werden!\n" + batch;
        ProcessService.StartFile(batch, $"\"{Path.Combine(model.Settings.NxInstallationPath, model.PostbuilderVersion)}\\\"");
        model.Last.LastPostbuilderVersion = model.PostbuilderVersion;
        model.Save();
        return "Postbuilder wurde gestartet.";
    }

    public string OpenMachineFolder()
    {
        var dir = Path.Combine(model.GetInstalledMachinesPath(), model.Machine);
        if (!Directory.Exists(dir)) return "Der Ordner konnte nicht geöffnet werden.\n" + dir;
        ProcessService.OpenFolder(dir);
        return "Ordner geöffnet.";
    }

    public string OpenFork()
    {
        var dir = Path.Combine(model.GetInstalledMachinesPath(), model.Machine);
        if (!File.Exists(model.Settings.ForkPath)) return "Fork ist nicht installiert oder der Pfad zur Fork.exe ist falsch.";
        if (!Directory.Exists(Path.Combine(dir, ".git"))) MessageBox.Show("Achtung: Kein Repository angelegt!", "Info");
        ProcessService.StartFile(model.Settings.ForkPath, $"\"{dir}\"");
        return "Fork wurde geöffnet.";
    }

    public string OpenVsCode()
    {
        var ppDir = Path.Combine(model.GetInstalledMachinesPath(), model.Machine, "postprocessor");
        if (!Directory.Exists(ppDir)) return "Der PP Ordner konnte nicht geöffnet werden da er nicht existiert.\n" + ppDir;
        ProcessService.StartFile("code", $"\"{ppDir}\"");
        return "VS Code wurde geöffnet.";
    }

    public string OpenLicenseFile()
    {
        var editor = ProcessService.FindEditor(model.Settings.Editor) ?? "notepad.exe";
        if (!File.Exists(model.Settings.LicencePath)) return "Die Lizenzdatei wurde nicht gefunden:\n" + model.Settings.LicencePath;
        ProcessService.StartFile(editor, $"\"{model.Settings.LicencePath}\"");
        return "Lizenzdatei geöffnet.";
    }

    public string OpenMainBatch()
    {
        return OpenFile("app/start_routine.bat");
    }

    public string OpenCustomerBatch()
    {
        return OpenFile(Path.Combine(model.Settings.CustomerEnvironmentPath, model.Customer, "5_Umgebung", model.VersionName, "start_apps", $"custom_nx_{model.Customer}.bat"));
    }

    public string OpenDeveloperBatch()
    {
        return OpenFile("app/user_settings.bat");
    }

    public string OpenFile(string path)
    {
        var editor = ProcessService.FindEditor(model.Settings.Editor) ?? "notepad.exe";
        if (!File.Exists(model.Settings.LicencePath)) return $"Die Datei '{path}' wurde nicht gefunden:";
        ProcessService.StartFile(editor, $"\"{path}\"");
        return "Datei geöffnet.";
    }

    public string StartLmTools()
    {
        if (!File.Exists(model.Settings.LicenceServerPath)) return "Der Pfad wurde nicht gefunden:\n" + model.Settings.LicenceServerPath;
        ProcessService.StartFile(model.Settings.LicenceServerPath);
        return "LMTools wurde gestartet.";
    }

    private string GetUgrafPath(string nxVersion)
    {
        var digits = int.TryParse(nxVersion.Replace("NX", "", StringComparison.OrdinalIgnoreCase), out var v) ? v : 9999;
        var sub = digits < 2206 ? Path.Combine("UGII", "ugraf.exe") : Path.Combine("NXBIN", "ugraf.exe");
        return Path.Combine(model.Settings.NxInstallationPath, nxVersion, sub);
    }

    private void CopyRoles(string nxVersion)
    {
        if (string.IsNullOrWhiteSpace(model.Settings.RolesPath)) return;
        var dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Siemens", nxVersion, "roles");
        Directory.CreateDirectory(dest);
        foreach (var role in model.Settings.RolesPath.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (File.Exists(role)) File.Copy(role, Path.Combine(dest, Path.GetFileName(role)), true);
        }
    }

    private static int Bool(int value) => value == 0 ? 0 : 1;
}
