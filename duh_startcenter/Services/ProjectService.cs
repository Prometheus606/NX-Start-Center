using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

using NXStartCenter;
namespace NXStartCenter.Services;

public sealed partial class ProjectService(AppModel model)
{
    public string CreateCustomerProject(string customer, string version, string machine, string order, string machineType, string controller)
    {
        if (string.IsNullOrWhiteSpace(customer) || string.IsNullOrWhiteSpace(version)) return "Kundenname und Version muss angegeben sein!";
        if (new[] { customer, version, machine, order }.Any(x => x.Contains(' '))) return "Leerzeichen sind nicht zulässig!";
        if (!NxVersionRegex().IsMatch(version)) return "Die Version muss das Format NXxxxx haben.";
        order = string.IsNullOrWhiteSpace(order) ? "0000" : order;

        var baseEnv = model.Settings.CustomerEnvironmentPath;
        var nxPath = model.Settings.NxInstallationPath;
        var customerPath = Path.Combine(baseEnv, customer);
        var installedMachinesDir = Path.Combine(customerPath, "5_Umgebung", version, "MACH", "resource", "library", "machine", "installed_machines");
        var machineDir = Path.Combine(installedMachinesDir, machine);
        var isNewCustomer = !Directory.Exists(customerPath);

        if (!Directory.Exists(baseEnv)) return $"Kundenumgebung existiert nicht:\n{baseEnv}";
        if (!string.IsNullOrWhiteSpace(machine) && Directory.Exists(machineDir)) return "Maschine bereits angelegt!";

        string[] dirs =
        [
            Path.Combine(customerPath, "1_Kundendaten", order),
            Path.Combine(customerPath, "2_Testdaten", order),
            Path.Combine(customerPath, "3_Auslieferung", order),
            Path.Combine(customerPath, "4_Calls", order),
            Path.Combine(customerPath, "5_Umgebung", version, "MACH", "resource", "usertools"),
            Path.Combine(customerPath, "5_Umgebung", version, "MACH", "resource", "library", "machine", "ascii"),
            Path.Combine(customerPath, "5_Umgebung", version, "MACH", "resource", "library", "machine", "inclass"),
            Path.Combine(customerPath, "6_Custom", order, version, "roles"),
            Path.Combine(customerPath, "6_Custom", order, version, "startup"),
            Path.Combine(customerPath, "7_Dokumentation", order),
            installedMachinesDir
        ];
        foreach (var dir in dirs) Directory.CreateDirectory(dir);

        if (isNewCustomer)
        {
            CopyDirectoryIfExists(Path.Combine(nxPath, version, "MACH", "resource", "library", "machine", "ascii"), Path.Combine(customerPath, "5_Umgebung", version, "MACH", "resource", "library", "machine", "ascii"));
            CopyDirectoryIfExists(Path.Combine(nxPath, version, "MACH", "resource", "library", "machine", "inclass"), Path.Combine(customerPath, "5_Umgebung", version, "MACH", "resource", "library", "machine", "inclass"));
        }

        if (!string.IsNullOrWhiteSpace(machine))
        {
            Directory.CreateDirectory(machineDir);
            Directory.CreateDirectory(Path.Combine(machineDir, "postprocessor"));
            Directory.CreateDirectory(Path.Combine(machineDir, "sample"));
            Directory.CreateDirectory(Path.Combine(machineDir, "cse_driver"));
            Directory.CreateDirectory(Path.Combine(machineDir, "documentation"));
            Directory.CreateDirectory(Path.Combine(machineDir, "graphics"));

            var typeId = model.MachineTypes.GetValueOrDefault(machineType, "MDM0101");
            File.WriteAllText(Path.Combine(machineDir, "README.md"), $"# {machine}  \n\nMaschine: {machine}  \nSteuerung: {controller}  \nFirma: {customer}  \nPost Configurator: -  \nNX-Version: {version}  \n");
            File.WriteAllText(Path.Combine(machineDir, machine + ".dat"), machine + ",${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + machine + "\\postprocessor\\" + machine + ".tcl,${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + machine + "\\postprocessor\\" + machine + ".def\nCSE_FILES, ${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + machine + "\\cse_driver\\" + controller + "\\" + machine + ".MCF");
            var line = "DATA|" + machine + "|" + typeId + "|" + machine + "|" + controller + "|Example|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + machine + "\\" + machine + ".dat|1.000000|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + machine + "\\graphics\\" + machine + "_SIM";
            File.WriteAllText(Path.Combine(machineDir, "add_to_machine_database.dat"), line);
            var asciiFile = Path.Combine(customerPath, "5_Umgebung", version, "MACH", "resource", "library", "machine", "ascii", "machine_database.dat");
            File.AppendAllText(asciiFile, Environment.NewLine + line);
        }

        model.RefreshAll();
        return "Neues Projekt wurde angelegt.";
    }

    private static void CopyDirectoryIfExists(string source, string target)
    {
        if (!Directory.Exists(source)) return;
        Directory.CreateDirectory(target);
        foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) Directory.CreateDirectory(dir.Replace(source, target));
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var dest = file.Replace(source, target);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(file, dest, true);
            File.SetAttributes(dest, FileAttributes.Normal);
        }
    }

    [GeneratedRegex("^NX\\d{2,4}$", RegexOptions.IgnoreCase)]
    private static partial Regex NxVersionRegex();
}
