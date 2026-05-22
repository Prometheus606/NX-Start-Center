using NXStartCenter;
using System.Diagnostics;
using System.IO;
using System.Windows;
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
        string oldRtbFilePath = Path.Combine(model.Settings.CustomerEnvironmentPath, model.Customer, model.EnvFolderName, model.VersionName, "UGII", "startup");
        RemoveFile(oldRtbFilePath);
        bool debug = model.Settings.StartNxWithDebug;
        string managed = model.StartNxManaged == true ? "portal_client" : "nx";
        var batch = Path.Combine(AppContext.BaseDirectory, "app", "start_routine.bat");
        if (!File.Exists(batch)) return "start_routine.bat wurde nicht gefunden.";
        if (String.IsNullOrEmpty(model.Customer.Trim()) || String.IsNullOrEmpty(model.VersionName.Trim()))
        {
            return "Kundenname und Version müssen angegeben werden!";
        }
        if (!Directory.Exists(Path.Combine(model.Settings.NxInstallationPath, model.VersionName)))
        {
            return "NX Version nicht installiert!";
        }
        bool loadFullResourceDir = model.Last.LastLoadFullResourceDir;
        if (model.Settings.Team == "CAM")
        {
            loadFullResourceDir = true;
        }
        var args =
            $"\"{model.Customer}\"" +
            $" \"{model.VersionName}\"" +
            $" \"{model.Last.LastLanguage}\"" +
            $" \"{model.Settings.CustomerEnvironmentPath}\"" +
            $" \"{model.Settings.NxInstallationPath}\"" +
            $" \"{debug}\"" +
            $" \"{model.Last.LastLoadPp}\" " +
            $"\"{model.Last.LastLoadInstalledMachines}\"" +
            $" \"{model.Last.LastLoadTool}\" " +
            $" \"{model.Last.LastLoadDevice}\" " +
            $" \"{model.Last.LastLoadFeed}\"" +
            $" \"{model.StartNxWithCloudLicense}\"" +
            $" \"{managed}\"" +
            $" \"{model.Settings.TemplateRoot}\"" +
            $" \"{model.Settings.TcPath}\"" +
            $" \"{loadFullResourceDir}\"";
        ProcessService.StartBatch(
    batch,
    args,
    AppContext.BaseDirectory,
    model.Settings.StartNxWithDebug
);
        model.Last.LastCustomer = model.Customer;
        model.Last.LastVersion = model.VersionName;
        model.Last.LastMachine = model.Machine;
        model.Save();
        return "NX wurde für die Kundenumgebung gestartet.";
    }

    public void RemoveFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
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
        if (model.Settings.ShowPullReminder)
        {
            ShowPullReminder();
        }
        
        ProcessService.StartFile(model.Settings.ForkPath, $"\"{dir}\"");
        return "Fork wurde geöffnet.";
    }

    public void ShowPullReminder()
    {
        MessageBox.Show(
            Application.Current.MainWindow,
            "Nicht vergessen das Projekt neu zu pullen!",
            "Info",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    public string OpenVsCode()
    {
        var ppDir = Path.Combine(model.GetInstalledMachinesPath(), model.Machine, "postprocessor");
        if (!Directory.Exists(ppDir)) return "Der PP Ordner konnte nicht geöffnet werden da er nicht existiert.\n" + ppDir;
        ProcessService.StartFile("code", $"\"{ppDir}\"");
        return "VS Code wurde geöffnet.";
    }

    public string OpenVsCodeAndFork()
    {
        string message = OpenVsCode();
        if (message != "VS Code wurde geöffnet.")
        {
            return message; 
        }
        if (model.Settings.OpenVsCodeWithFork)
        {
            message = OpenFork();          
        }
        return message;
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
        return OpenFile(Path.Combine(model.Settings.CustomerEnvironmentPath, model.Customer, model.EnvFolderName, model.VersionName, "start_apps", $"custom_nx_{model.Customer}.bat"));
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
