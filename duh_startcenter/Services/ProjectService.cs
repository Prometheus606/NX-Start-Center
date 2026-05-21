using NXStartCenter;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Windows.Controls;



namespace NXStartCenter.Services;

public sealed partial class ProjectService(AppModel model)
{
    private string customer = "";
    private string version = "";
    private string machine = "";
    private string order = "";
    private string machineType = "";
    private string controller = "";
    private string baseEnv = "";
    private string nxPath = "";
    private string customerPath = "";
    private string installedMachinesDir = "";
    private string machineDir = "";
    private string TemplateRootDir = "";
    private bool isNewCustomer = true;

    public string CreateCustomerProject(string customer, string version, string machine, string order, string machineType, string controller, bool complexEnvRequired)
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

        this.customer = customer;
        this.version = version;
        this.machine = machine;
        this.machineType = machineType;
        this.controller = controller;
        this.order = order;
        this.baseEnv = baseEnv;
        this.nxPath = nxPath;
        this.customerPath = customerPath;
        this.installedMachinesDir = installedMachinesDir;
        this.machineDir = machineDir;
        this.isNewCustomer = isNewCustomer;
        TemplateRootDir = model.Settings.TemplateRoot;

        if (isNewCustomer)
        {
            if (complexEnvRequired)
                CreateComplexNxEnvironment();
            else
                CreateSimpleNxEnvironment();
        }       

        if (!string.IsNullOrWhiteSpace(machine))
        {
            CreateNewMachine();
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

    public void CreateNewMachine()
    {

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
    }

    public void CreateSimpleNxEnvironment()
    {
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
    }


    public void CreateComplexNxEnvironment()
    {
        string umgebung = "5_Umgebung";

        string ugiiBaseDir = Path.Combine(nxPath, version);
        string customerRoot = Path.Combine(customerPath, customer);
        string envRoot = Path.Combine(customerRoot, umgebung, version);
        string machRoot = Path.Combine(envRoot, "MACH");
        string resourceRoot = Path.Combine(machRoot, "resource");

        // Basisordner
        CreateDirs(
            customerRoot,
            Path.Combine(customerRoot, "1_Kundendaten"),
            Path.Combine(customerRoot, "2_Testdaten"),
            Path.Combine(customerRoot, "2_Testdaten", "Temp"),
            Path.Combine(customerRoot, "2_Testdaten", "Temp", "NX"),
            Path.Combine(customerRoot, "2_Testdaten", "Shop_Doc"),
            Path.Combine(customerRoot, "4_Calls"),
            Path.Combine(customerRoot, "6_Custom"),
            Path.Combine(customerRoot, "7_Dokumentation"),
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
            Path.Combine(resourceRoot, $"ug_library_{customer}"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "user_def_event"),
            Path.Combine(resourceRoot, $"user_def_event_{customer}"));

        // Library: device
        CreateDirs(
            Path.Combine(resourceRoot, "library", "machine", "installed_machines"),
            Path.Combine(resourceRoot, "library", "device", $"ascii_{customer}"),
            Path.Combine(resourceRoot, "library", "device", $"graphics_{customer}")
        );

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "device", "ascii", "device_database.dat"),
            Path.Combine(resourceRoot, "library", "device", $"ascii_{customer}", "device_database.dat"));

        // Library: feeds_speeds
        string feedsTarget = Path.Combine(resourceRoot, "library", "feeds_speeds", $"ascii_{customer}");
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

        // Library: machine
        CreateDirs(
            Path.Combine(resourceRoot, "library", "machine", $"ascii_{customer}"),
            Path.Combine(resourceRoot, "library", "machine", $"installed_machines_{customer}")
        );

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "machine", "ascii", "machine_database.dat"),
            Path.Combine(resourceRoot, "library", "machine", $"ascii_{customer}", "machine_database.dat"));

        // Library: tool
        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "tool", "ascii"),
            Path.Combine(resourceRoot, "library", "tool", $"ascii_{customer}"));

        CreateDirs(Path.Combine(resourceRoot, "library", "tool", $"graphics_{customer}"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "tool", "metric"),
            Path.Combine(resourceRoot, "library", "tool", $"metric_{customer}"));

        // Fixture Automation nur NX2512
        if (version.Equals("NX2512", StringComparison.OrdinalIgnoreCase))
        {
            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "fixture_automation", "ascii"),
                Path.Combine(resourceRoot, "library", "fixture_automation", $"ascii_{customer}"));

            CreateDirs(Path.Combine(resourceRoot, "library", "fixture_automation", $"graphics_{customer}"));

            CopyDirectoryIfMissingOrEmpty(
                Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "fixture_automation", "metric"),
                Path.Combine(resourceRoot, "library", "fixture_automation", $"metric_{customer}"));
        }

        // Einzeldateien mit kundenspezifischem Namen
        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "machining_knowledge", "machining_knowledge.dat"),
            Path.Combine(resourceRoot, "machining_knowledge", $"machining_knowledge_{customer}.dat"));

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "machining_knowledge", "machining_knowledge.xml"),
            Path.Combine(resourceRoot, "machining_knowledge", $"machining_knowledge_{customer}.xml"));

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "machining_knowledge", "machining_knowledge_tc.xml"),
            Path.Combine(resourceRoot, "machining_knowledge", $"machining_knowledge_{customer}_tc.xml"));

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "machining_knowledge", "machining_knowledge_part_planner.dat"),
            Path.Combine(resourceRoot, "machining_knowledge", $"machining_knowledge_{customer}_tc.dat"));

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "postprocessor", "template_post.dat"),
            Path.Combine(resourceRoot, "postprocessor", $"template_post_{customer}.dat"));

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "shop_doc", "shop_doc.dat"),
            Path.Combine(resourceRoot, "shop_doc", $"shop_doc_{customer}.dat"));

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "template_set", "cam_general.opt"),
            Path.Combine(resourceRoot, "template_set", $"cam_{customer}_native.opt"));

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "template_set", "cam_teamcenter_general.opt"),
            Path.Combine(resourceRoot, "template_set", $"cam_{customer}_tc.opt"));

        // configuration
        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "configuration"),
            Path.Combine(resourceRoot, "configuration"));

        CreateCamConfigFiles(resourceRoot, customer);

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
            Path.Combine(resourceRoot, $"template_dir_{customer}"));

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
            Path.Combine(TemplateRootDir, "Vorlage", version, "CustomerDefaults", "Site"),
            Path.Combine(envRoot, "CustomerDefaults", "Site"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(TemplateRootDir, "Vorlage", version, "CustomerDefaults", "Group"),
            Path.Combine(envRoot, "CustomerDefaults", "Group"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(TemplateRootDir, "Vorlage", version, "CustomerDefaults", "User"),
            Path.Combine(envRoot, "CustomerDefaults", "User"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(TemplateRootDir, "Vorlage", version, "CustomerDefaults", "EarlyAccessFeature"),
            Path.Combine(envRoot, "CustomerDefaults", "EarlyAccessFeature"));

        // custom_nx_CUSTOMER.bat
        CopyFileIfMissing(
            Path.Combine(TemplateRootDir, "Vorlage", "start_apps", "custom_nx.bat"),
            Path.Combine(envRoot, "start_apps", $"custom_nx_{customer}.bat"));
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
