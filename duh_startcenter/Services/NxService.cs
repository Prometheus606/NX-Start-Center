using Microsoft.Win32;
using NXStartCenter;
using System.Diagnostics;
using System.IO;
using System.Windows;
using NXStartCenter.Model;

namespace NXStartCenter.Services;


public sealed class NxService(AppModel model)
{
    public string StartNativeNx()
    {
        //CopyRoles(model.NativeVersion);
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
        GeneralService.RemoveFile(oldRtbFilePath);
        bool debug = model.Settings.StartNxWithDebug;
        string managed = model.StartNxManaged == true ? "portal_client" : "nx";
        var batch = Path.Combine(AppContext.BaseDirectory, "Batch_files", "start_routine.bat");
        if (!File.Exists(batch)) return "start_routine.bat wurde nicht gefunden.";
        if (String.IsNullOrEmpty(model.Customer.Trim()) || String.IsNullOrEmpty(model.VersionName.Trim()))
        {
            return "Kundenname und Version müssen angegeben werden!";
        }
        if (!Directory.Exists(Path.Combine(model.Settings.NxInstallationPath, model.VersionName)))
        {
            return "NX Version nicht installiert!";
        }
        bool isPPUser = model.Settings.Team == "PP";
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
            $" \"{isPPUser}\"" +
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
}
