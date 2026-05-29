using NXStartCenter;
using NXStartCenter.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Windows.Controls;



namespace NXStartCenter.Services;

public sealed partial class NewProjectService(AppModel model)
{
    private string newCustomerName = "";
    private string newVersion = "";
    private string newMachineName = "";
    private string newOrderNumber = "";
    private string newMachineType = "";
    private string newController = "";
    private string baseEnv = "";
    private string nxPath = "";
    private string customerPath = "";
    private string installedMachinesDir = "";
    private string machineDir = "";
    private string TemplateRootDir = "";
    private bool isNewCustomer = true;
    private string envFolderName = "";

    public string CreateOrExtendCustomerEnvironment(string newCustomerName, string newVersion, string newMachineName, string newOrderNumber, string newMachineType, string newController, bool complexEnvRequired)
    {
        if (string.IsNullOrWhiteSpace(newCustomerName) || string.IsNullOrWhiteSpace(newVersion)) return "Kundenname und Version muss angegeben sein!";
        if (new[] { newCustomerName, newVersion, newMachineName, newOrderNumber }.Any(x => x.Contains(' '))) return "Leerzeichen sind nicht zulässig!";
        if (!NxVersionRegex().IsMatch(newVersion)) return "Die Version muss das Format NXxxxx haben.";
        newOrderNumber = string.IsNullOrWhiteSpace(newOrderNumber) ? "0000" : newOrderNumber;

        var baseEnv = model.Settings.CustomerEnvironmentPath;
        var nxPath = model.Settings.NxInstallationPath;
        var customerPath = Path.Combine(baseEnv, newCustomerName);
        var isNewCustomer = !Directory.Exists(customerPath);
        var isNewNxVersion = !Directory.Exists(Path.Combine(customerPath, model.EnvFolderName, newVersion));

        this.newCustomerName = newCustomerName;
        this.newVersion = newVersion;
        this.newMachineName = newMachineName;
        this.newMachineType = newMachineType;
        this.newController = newController;
        this.newOrderNumber = newOrderNumber;
        this.baseEnv = baseEnv;
        this.nxPath = nxPath;
        this.customerPath = customerPath;
        this.installedMachinesDir = model.GetInstalledMachinesPath(newCustomerName, newVersion);
        this.machineDir = Path.Combine(installedMachinesDir, newMachineName);

        if (!Directory.Exists(baseEnv)) return $"Kundenumgebung existiert nicht:\n{baseEnv}";
        if (!string.IsNullOrWhiteSpace(newMachineName) && Directory.Exists(machineDir)) return "Maschine bereits angelegt!";
        TemplateRootDir = model.Settings.TemplateRoot;
        envFolderName = model.EnvFolderName;

        try
        {

            if (isNewCustomer || (!isNewCustomer && isNewNxVersion))
            {
                if (complexEnvRequired)
                    CreateComplexNxEnvironment();
                else
                    CreateSimpleNxEnvironment();
            }       

            if (!string.IsNullOrWhiteSpace(newMachineName))
            {
                CreateNewMachine();
            }
        }
        catch (Exception ex)
        {

            return $"Fehler beim erzeugen: {ex.Message}";
        }
        model.RefreshAll();
        return "Neues Projekt wurde angelegt.";
    }

    private string GetMachineDatabaseFile()
    {
        string basePath = Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "library", "machine");
        if (Path.Exists(Path.Combine(basePath, $"ascii_{newCustomerName}")))
        {
            return Path.Combine(basePath, $"ascii_{newCustomerName}", "machine_database.dat");
        }
        return Path.Combine(basePath, "ascii", "machine_database.dat");
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

    public void CreateNewMachine()
    {

        if (string.IsNullOrWhiteSpace(newMachineName)) { return; }

        this.installedMachinesDir = model.GetInstalledMachinesPath(newCustomerName, newVersion);
        this.machineDir = Path.Combine(installedMachinesDir, newMachineName);

        Directory.CreateDirectory(machineDir);
        Directory.CreateDirectory(Path.Combine(machineDir, "postprocessor"));
        Directory.CreateDirectory(Path.Combine(machineDir, "sample"));
        Directory.CreateDirectory(Path.Combine(machineDir, "cse_driver"));
        Directory.CreateDirectory(Path.Combine(machineDir, "documentation"));
        Directory.CreateDirectory(Path.Combine(machineDir, "graphics"));

        var typeId = model.MachineTypes.GetValueOrDefault(newMachineType, "MDM0101");
        File.WriteAllText(Path.Combine(machineDir, "README.md"), $"# {newMachineName}  \n\nMaschine: {newMachineName}  \nSteuerung: {newController}  \nFirma: {newCustomerName}  \nPost Configurator: -  \nNX-Version: {newVersion}  \n");
        File.WriteAllText(Path.Combine(machineDir, newMachineName + ".dat"), newMachineName + ",${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\postprocessor\\" + newMachineName + ".tcl,${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\postprocessor\\" + newMachineName + ".def\nCSE_FILES, ${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\cse_driver\\" + newController + "\\" + newMachineName + ".MCF");
        var line = "DATA|" + newMachineName + "|" + typeId + "|" + newMachineName + "|" + newController + "|Example|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\" + newMachineName + ".dat|1.000000|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\graphics\\" + newMachineName + "_SIM";
        File.WriteAllText(Path.Combine(machineDir, "add_to_machine_database.dat"), line);
        var asciiFile = GetMachineDatabaseFile();
        if (File.Exists(asciiFile))
        {
            File.SetAttributes(asciiFile, FileAttributes.Normal);
            File.AppendAllText(asciiFile, Environment.NewLine + line);
        }
        
    }

    public void CreateSimpleNxEnvironment()
    {

        string[] dirs =
        [
            Path.Combine(customerPath, "1_Kundendaten", newOrderNumber),
            Path.Combine(customerPath, "2_Testdaten", newOrderNumber),
            Path.Combine(customerPath, "3_Auslieferung", newOrderNumber),
            Path.Combine(customerPath, "4_Calls", newOrderNumber),
            Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "usertools"),
            Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "library", "machine", "ascii"),
            Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "library", "machine", "inclass"),
            Path.Combine(customerPath, "6_Custom", newOrderNumber, newVersion, "roles"),
            Path.Combine(customerPath, "6_Custom", newOrderNumber, newVersion, "startup"),
            Path.Combine(customerPath, "7_Dokumentation", newOrderNumber),
            installedMachinesDir
        ];
        foreach (var dir in dirs) Directory.CreateDirectory(dir);

        if (isNewCustomer)
        {
            CopyDirectoryIfExists(Path.Combine(nxPath, newVersion, "MACH", "resource", "library", "machine", "ascii"), Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "library", "machine", "ascii"));
            CopyDirectoryIfExists(Path.Combine(nxPath, newVersion, "MACH", "resource", "library", "machine", "inclass"), Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "library", "machine", "inclass"));
        }

        if (!string.IsNullOrWhiteSpace(newMachineName))
        {
            Directory.CreateDirectory(machineDir);
            Directory.CreateDirectory(Path.Combine(machineDir, "postprocessor"));
            Directory.CreateDirectory(Path.Combine(machineDir, "sample"));
            Directory.CreateDirectory(Path.Combine(machineDir, "cse_driver"));
            Directory.CreateDirectory(Path.Combine(machineDir, "documentation"));
            Directory.CreateDirectory(Path.Combine(machineDir, "graphics"));

            var typeId = model.MachineTypes.GetValueOrDefault(newMachineType, "MDM0101");
            File.WriteAllText(Path.Combine(machineDir, "README.md"), $"# {newMachineName}  \n\nMaschine: {newMachineName}  \nSteuerung: {newController}  \nFirma: {newCustomerName}  \nPost Configurator: -  \nNX-Version: {newVersion}  \n");
            File.WriteAllText(Path.Combine(machineDir, newMachineName + ".dat"), newMachineName + ",${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\postprocessor\\" + newMachineName + ".tcl,${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\postprocessor\\" + newMachineName + ".def\nCSE_FILES, ${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\cse_driver\\" + newController + "\\" + newMachineName + ".MCF");
            var line = "DATA|" + newMachineName + "|" + typeId + "|" + newMachineName + "|" + newController + "|Example|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\" + newMachineName + ".dat|1.000000|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\graphics\\" + newMachineName + "_SIM";
            File.WriteAllText(Path.Combine(machineDir, "add_to_machine_database.dat"), line);
            var asciiFile = Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "library", "machine", "ascii", "machine_database.dat");
            File.AppendAllText(asciiFile, Environment.NewLine + line);
        }
    }


    public void CreateComplexNxEnvironment()
    {
        string ugiiBaseDir = Path.Combine(nxPath, newVersion);
        if (!Path.Exists(ugiiBaseDir)) throw new Exception("NX Version nicht installiert!");
        string envRoot = Path.Combine(customerPath, envFolderName, newVersion);
        string machRoot = Path.Combine(envRoot, "MACH");
        string resourceRoot = Path.Combine(machRoot, "resource");

            // Basisordner
            CreateDirs(
                customerPath,
                Path.Combine(customerPath, "1_Kundendaten"),
                Path.Combine(customerPath, "2_Testdaten"),
                Path.Combine(customerPath, "2_Testdaten", "Temp"),
                Path.Combine(customerPath, "2_Testdaten", "Temp", "NX"),
                Path.Combine(customerPath, "2_Testdaten", "Shop_Doc"),
                Path.Combine(customerPath, "4_Calls"),
                Path.Combine(customerPath, "6_Custom"),
                Path.Combine(customerPath, "7_Dokumentation"),
                envRoot,
                machRoot,
                Path.Combine(envRoot, "Reuse_Library"),
                Path.Combine(envRoot, "UGII"),
                Path.Combine(envRoot, "UGII", "menus"),
                Path.Combine(envRoot, "start_apps"),
                resourceRoot,
                Path.Combine(machRoot, "CAM_POST_OUTPUT_DIR"),
                Path.Combine(machRoot, "CAM_SETUP_ROOT_DIR")
            );

            // Große Ordnerkopien
            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource"),
                resourceRoot,
                Path.Combine(resourceRoot, "library"));

            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "templates"),
                Path.Combine(machRoot, "templates"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "UGII", "templates", "ugs_model_templates.pax"),
                Path.Combine(machRoot, "templates", "ugs_model_templates.pax"));

            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "ug_library"),
                Path.Combine(resourceRoot, $"ug_library_{newCustomerName}"));

            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "user_def_event"),
                Path.Combine(resourceRoot, $"user_def_event_{newCustomerName}"));

            // Library: device
            CreateDirs(
                Path.Combine(resourceRoot, "library", "machine", "installed_machines"),
                Path.Combine(resourceRoot, "library", "device", $"ascii_{newCustomerName}"),
                Path.Combine(resourceRoot, "library", "device", $"graphics_{newCustomerName}")
            );

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "device", "ascii", "device_database.dat"),
                Path.Combine(resourceRoot, "library", "device", $"ascii_{newCustomerName}", "device_database.dat"));

            // Library: feeds_speeds
            string feedsTarget = Path.Combine(resourceRoot, "library", "feeds_speeds", $"ascii_{newCustomerName}");
            CreateDirs(feedsTarget);

            foreach (string file in new[]
            {
                "cut_methods.dat",
                "feeds_speeds.dat",
                "machining_data.dat",
                "part_materials.dat",
                "process_force_parameters.dat",
                "tool_machining_data.dat",
                "tool_materials.dat"
            })
            {
                CopyFileIfMissing(
                    Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "feeds_speeds", "ascii", file),
                    Path.Combine(feedsTarget, file));
            }

            // Library: newMachineName
            CreateDirs(
                Path.Combine(resourceRoot, "library", "machine", $"ascii_{newCustomerName}"),
                Path.Combine(resourceRoot, "library", "machine", $"installed_machines_{newCustomerName}")
            );

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "machine", "ascii", "machine_database.dat"),
                Path.Combine(resourceRoot, "library", "machine", $"ascii_{newCustomerName}", "machine_database.dat"));

            // Library: tool
            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "tool", "ascii"),
                Path.Combine(resourceRoot, "library", "tool", $"ascii_{newCustomerName}"));

            CreateDirs(Path.Combine(resourceRoot, "library", "tool", $"graphics_{newCustomerName}"));

            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "tool", "metric"),
                Path.Combine(resourceRoot, "library", "tool", $"metric_{newCustomerName}"));

            // Fixture Automation nur NX2512
            if (newVersion.Equals("NX2512", StringComparison.OrdinalIgnoreCase))
            {
                CopyDirectoryIfMissingOrEmpty(
                    Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "fixture_automation", "ascii"),
                    Path.Combine(resourceRoot, "library", "fixture_automation", $"ascii_{newCustomerName}"));

                CreateDirs(Path.Combine(resourceRoot, "library", "fixture_automation", $"graphics_{newCustomerName}"));

                CopyDirectoryIfMissingOrEmpty(
                    Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "fixture_automation", "metric"),
                    Path.Combine(resourceRoot, "library", "fixture_automation", $"metric_{newCustomerName}"));
            }

            // Einzeldateien mit kundenspezifischem Namen
            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "machining_knowledge", "machining_knowledge.dat"),
                Path.Combine(resourceRoot, "machining_knowledge", $"machining_knowledge_{newCustomerName}.dat"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "machining_knowledge", "machining_knowledge.xml"),
                Path.Combine(resourceRoot, "machining_knowledge", $"machining_knowledge_{newCustomerName}.xml"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "machining_knowledge", "machining_knowledge_tc.xml"),
                Path.Combine(resourceRoot, "machining_knowledge", $"machining_knowledge_{newCustomerName}_tc.xml"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "machining_knowledge", "machining_knowledge_part_planner.dat"),
                Path.Combine(resourceRoot, "machining_knowledge", $"machining_knowledge_{newCustomerName}_tc.dat"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "postprocessor", "template_post.dat"),
                Path.Combine(resourceRoot, "postprocessor", $"template_post_{newCustomerName}.dat"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "shop_doc", "shop_doc.dat"),
                Path.Combine(resourceRoot, "shop_doc", $"shop_doc_{newCustomerName}.dat"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "template_set", "cam_general.opt"),
                Path.Combine(resourceRoot, "template_set", $"cam_{newCustomerName}_native.opt"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "template_set", "cam_teamcenter_general.opt"),
                Path.Combine(resourceRoot, "template_set", $"cam_{newCustomerName}_tc.opt"));

            // configuration
            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "configuration"),
                Path.Combine(resourceRoot, "configuration"));

            CreateCamConfigFiles(resourceRoot, newCustomerName);

            // auxiliary
            string auxiliaryMetric = Path.Combine(machRoot, "auxiliary", "tagging", "metric");
            CreateDirs(auxiliaryMetric);

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "auxiliary", "tagging", "metric", "tagging.dat"),
                Path.Combine(auxiliaryMetric, "tagging.dat"));

            CopyFileIfMissing(
                Path.Combine(ugiiBaseDir, "MACH", "auxiliary", "tagging", "metric", "tagging_fea.dat"),
                Path.Combine(auxiliaryMetric, "tagging_fea.dat"));

            // template_dir
            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "template_dir"),
                Path.Combine(resourceRoot, $"template_dir_{newCustomerName}"));

            // duh_tools
            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(TemplateRootDir, "Vorlage", "duh_tools"),
                Path.Combine(envRoot, "duh_tools"));

            // custom_dirs.dat
            CopyFileIfMissing(
                Path.Combine(TemplateRootDir, "Vorlage", "custom_dirs.dat"),
                Path.Combine(envRoot, "UGII", "menus", "custom_dirs.dat"));

            // ugii_env.dat
            CopyFileIfMissing(
                Path.Combine(TemplateRootDir, "Vorlage", "ugii_env.dat"),
                Path.Combine(envRoot, "UGII", "ugii_env.dat"));

            // CustomerDefaults
            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(TemplateRootDir, "Vorlage", newVersion, "CustomerDefaults", "Site"),
                Path.Combine(envRoot, "CustomerDefaults", "Site"));

            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(TemplateRootDir, "Vorlage", newVersion, "CustomerDefaults", "Group"),
                Path.Combine(envRoot, "CustomerDefaults", "Group"));

            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(TemplateRootDir, "Vorlage", newVersion, "CustomerDefaults", "User"),
                Path.Combine(envRoot, "CustomerDefaults", "User"));

            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(TemplateRootDir, "Vorlage", newVersion, "CustomerDefaults", "EarlyAccessFeature"),
                Path.Combine(envRoot, "CustomerDefaults", "EarlyAccessFeature"));

            // custom_nx_CUSTOMER.bat
            CopyFileIfMissing(
                Path.Combine(TemplateRootDir, "Vorlage", "start_apps", "custom_nx.bat"),
                Path.Combine(envRoot, "start_apps", $"custom_nx_{newCustomerName}.bat"));

    }

    private static void CreateCamConfigFiles(string resourceRoot, string customer)
    {
        string basePath = Path.Combine(resourceRoot, "configuration");

        CreateCamConfigFile(
            Path.Combine(basePath, "cam_general.dat"),
            Path.Combine(basePath, $"cam_{customer}_native.dat"),
            customer,
            "_native");

        CreateCamConfigFile(
            Path.Combine(basePath, "cam_teamcenter_ascii_library.dat"),
            Path.Combine(basePath, $"cam_{customer}_tc.dat"),
            customer,
            "_tc");
    }

    private static void CreateCamConfigFile(
        string sourceFile,
        string targetFile,
        string customer,
        string suffix)
    {
        if (!File.Exists(sourceFile))
            return;

        if (File.Exists(targetFile))
            return;

        Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);

        var headerLines = new[]
        {
            $"TEMPLATE_OPERATION,${{UGII_CAM_TEMPLATE_SET_DIR}}cam_{customer}{suffix}.opt",
            $"TEMPLATE_DOCUMENTATION,${{UGII_CAM_SHOP_DOC_DIR}}shop_doc_{customer}.dat",
            $"TEMPLATE_POST,${{UGII_CAM_POST_DIR}}template_post_{customer}.dat"
        };

        var sourceLinesFromLine4 = File.ReadAllLines(sourceFile).Skip(3);

        File.WriteAllLines(targetFile, headerLines.Concat(sourceLinesFromLine4));
    }

    private static void CreateDirs(params string[] directories)
    {
        foreach (string dir in directories)
        {
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);
        }
    }

    private static void CopyFileIfMissing(string sourceFile, string targetFile)
    {
        if (!File.Exists(sourceFile))
            return;

        if (File.Exists(targetFile))
            return;

        Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
        File.Copy(sourceFile, targetFile, overwrite: false);
    }

    private static void CopyDirectoryIfMissingOrEmpty(
        string sourceDirectory,
        string targetDirectory,
        string? existenceCheckPath = null)
    {
        string checkPath = existenceCheckPath ?? targetDirectory;

        if (!Directory.Exists(sourceDirectory))
            return;

        if (Directory.Exists(checkPath))
            return;

        CopyDirectory(sourceDirectory, targetDirectory);
    }

    private static void CopyDirectory(string sourceDirectory, string targetDirectory)
    {
        Directory.CreateDirectory(targetDirectory);

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string targetFile = Path.Combine(targetDirectory, Path.GetFileName(file));
            File.Copy(file, targetFile, overwrite: false);
        }

        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string targetSubDirectory = Path.Combine(targetDirectory, Path.GetFileName(directory));
            CopyDirectory(directory, targetSubDirectory);
        }
    }
}
