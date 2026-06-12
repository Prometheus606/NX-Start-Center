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

    private readonly List<string> createdDirectories = [];
    private readonly List<string> createdFiles = [];
    private readonly Dictionary<string, string> fileBackups = [];

    public string CreateOrExtendCustomerEnvironment(string newCustomerName, string newVersion, string newMachineName, string newOrderNumber, string newMachineType, string newController, bool complexEnvRequired)
    {
        createdDirectories.Clear();
        createdFiles.Clear();
        fileBackups.Clear();

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
        this.isNewCustomer = isNewCustomer;

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
            Rollback();
            return $"Fehler beim Erzeugen: {ex.Message}";
        }

        model.RefreshAll();
        return "Neues Projekt wurde angelegt.";
    }

    private void Rollback()
    {
        foreach (var file in createdFiles.AsEnumerable().Reverse())
        {
            try
            {
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }
            catch
            {
                // Rollback darf den Originalfehler nicht überdecken.
            }
        }

        foreach (var backup in fileBackups)
        {
            try
            {
                CreateDirTracked(Path.GetDirectoryName(backup.Key)!);
                File.SetAttributes(backup.Key, FileAttributes.Normal);
                File.WriteAllText(backup.Key, backup.Value);
            }
            catch
            {
                // Rollback darf den Originalfehler nicht überdecken.
            }
        }

        foreach (var dir in createdDirectories.AsEnumerable().Reverse())
        {
            try
            {
                if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                    Directory.Delete(dir);
            }
            catch
            {
                // Rollback darf den Originalfehler nicht überdecken.
            }
        }

        createdDirectories.Clear();
        createdFiles.Clear();
        fileBackups.Clear();
    }

    private void CreateDirTracked(string dir)
    {
        if (string.IsNullOrWhiteSpace(dir))
            return;

        var fullPath = Path.GetFullPath(dir);
        var directoriesToCreate = new Stack<string>();

        var current = fullPath;
        while (!string.IsNullOrWhiteSpace(current) && !Directory.Exists(current))
        {
            directoriesToCreate.Push(current);
            current = Path.GetDirectoryName(current);
        }

        while (directoriesToCreate.Count > 0)
        {
            var directory = directoriesToCreate.Pop();
            Directory.CreateDirectory(directory);

            if (!createdDirectories.Contains(directory))
                createdDirectories.Add(directory);
        }
    }

    private void TrackCreatedFile(string file)
    {
        if (!createdFiles.Contains(file) && !fileBackups.ContainsKey(file))
            createdFiles.Add(file);
    }

    private void BackupFileIfExists(string file)
    {
        if (File.Exists(file) && !fileBackups.ContainsKey(file))
            fileBackups[file] = File.ReadAllText(file);
    }

    private void WriteAllTextTracked(string file, string content)
    {
        BackupFileIfExists(file);
        CreateDirTracked(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, content);

        if (!fileBackups.ContainsKey(file))
            TrackCreatedFile(file);
    }

    private void AppendAllTextTracked(string file, string content)
    {
        BackupFileIfExists(file);
        CreateDirTracked(Path.GetDirectoryName(file)!);
        File.AppendAllText(file, content);

        if (!fileBackups.ContainsKey(file))
            TrackCreatedFile(file);
    }

    private void CopyFileTracked(string sourceFile, string targetFile, bool overwrite)
    {
        if (!File.Exists(sourceFile))
            throw new FileNotFoundException($"Die Quelldatei '{sourceFile}' wurde nicht gefunden.", sourceFile);

        if (File.Exists(targetFile))
        {
            if (!overwrite)
                return;

            BackupFileIfExists(targetFile);
            File.SetAttributes(targetFile, FileAttributes.Normal);
        }

        CreateDirTracked(Path.GetDirectoryName(targetFile)!);
        File.Copy(sourceFile, targetFile, overwrite);

        if (!fileBackups.ContainsKey(targetFile))
            TrackCreatedFile(targetFile);

        File.SetAttributes(targetFile, FileAttributes.Normal);
    }

    private void CopyFileIfMissing(string sourceFile, string targetFile)
    {
        if (!File.Exists(sourceFile))
            throw new FileNotFoundException($"Die Quelldatei '{sourceFile}' wurde nicht gefunden.", sourceFile);

        if (File.Exists(targetFile))
            return;

        CopyFileTracked(sourceFile, targetFile, overwrite: false);
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

    private void CopyDirectoryIfExists(string source, string target)
    {
        if (!Directory.Exists(source))
            throw new DirectoryNotFoundException($"Das Quellverzeichnis '{source}' wurde nicht gefunden.");

        CopyDirectory(source, target, overwriteFiles: true);
    }

    [GeneratedRegex("^NX\\d{2,4}$", RegexOptions.IgnoreCase)]
    private static partial Regex NxVersionRegex();

    public void CreateNewMachine()
    {
        if (string.IsNullOrWhiteSpace(newMachineName)) { return; }

        this.installedMachinesDir = model.GetInstalledMachinesPath(newCustomerName, newVersion);
        this.machineDir = Path.Combine(installedMachinesDir, newMachineName);

        CreateDirTracked(machineDir);
        CreateDirTracked(Path.Combine(machineDir, "postprocessor"));
        CreateDirTracked(Path.Combine(machineDir, "sample"));
        CreateDirTracked(Path.Combine(machineDir, "cse_driver"));
        CreateDirTracked(Path.Combine(machineDir, "documentation"));
        CreateDirTracked(Path.Combine(machineDir, "graphics"));

        var typeId = model.MachineTypes.GetValueOrDefault(newMachineType, "MDM0101");

        WriteAllTextTracked(
            Path.Combine(machineDir, "README.md"),
            $"# {newMachineName}  \n\nMaschine: {newMachineName}  \nSteuerung: {newController}  \nFirma: {newCustomerName}  \nPost Configurator: -  \nNX-Version: {newVersion}  \n");

        WriteAllTextTracked(
            Path.Combine(machineDir, newMachineName + ".dat"),
            newMachineName + ",${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\postprocessor\\" + newMachineName + ".tcl,${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\postprocessor\\" + newMachineName + ".def\nCSE_FILES, ${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\cse_driver\\" + newController + "\\" + newMachineName + ".MCF");

        var line = "DATA|" + newMachineName + "|" + typeId + "|" + newMachineName + "|" + newController + "|Example|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\" + newMachineName + ".dat|1.000000|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + newMachineName + "\\graphics\\" + newMachineName + "_SIM";

        WriteAllTextTracked(Path.Combine(machineDir, "add_to_machine_database.dat"), line);

        var asciiFile = GetMachineDatabaseFile();
        if (File.Exists(asciiFile))
        {
            File.SetAttributes(asciiFile, FileAttributes.Normal);
            AppendAllTextTracked(asciiFile, Environment.NewLine + line);
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

        foreach (var dir in dirs)
            CreateDirTracked(dir);

        if (isNewCustomer)
        {
            CopyDirectoryIfExists(
                Path.Combine(nxPath, newVersion, "MACH", "resource", "library", "machine", "ascii"),
                Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "library", "machine", "ascii"));

            CopyDirectoryIfExists(
                Path.Combine(nxPath, newVersion, "MACH", "resource", "library", "machine", "inclass"),
                Path.Combine(customerPath, model.EnvFolderName, newVersion, "MACH", "resource", "library", "machine", "inclass"));
        }

        if (!string.IsNullOrWhiteSpace(newMachineName))
        {
            CreateNewMachine();
        }
    }

    public void CreateComplexNxEnvironment()
    {
        string ugiiBaseDir = Path.Combine(nxPath, newVersion);
        if (!Path.Exists(ugiiBaseDir)) throw new Exception("NX Version nicht installiert!");

        string envRoot = Path.Combine(customerPath, envFolderName, newVersion);
        string machRoot = Path.Combine(envRoot, "MACH");
        string resourceRoot = Path.Combine(machRoot, "resource");

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

        CreateDirs(
            Path.Combine(resourceRoot, "library", "machine", "installed_machines"),
            Path.Combine(resourceRoot, "library", "device", $"ascii_{newCustomerName}"),
            Path.Combine(resourceRoot, "library", "device", $"graphics_{newCustomerName}")
        );

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "device", "ascii", "device_database.dat"),
            Path.Combine(resourceRoot, "library", "device", $"ascii_{newCustomerName}", "device_database.dat"));

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

        CreateDirs(
            Path.Combine(resourceRoot, "library", "machine", $"ascii_{newCustomerName}"),
            Path.Combine(resourceRoot, "library", "machine", $"installed_machines_{newCustomerName}")
        );

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "machine", "ascii", "machine_database.dat"),
            Path.Combine(resourceRoot, "library", "machine", $"ascii_{newCustomerName}", "machine_database.dat"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "tool", "ascii"),
            Path.Combine(resourceRoot, "library", "tool", $"ascii_{newCustomerName}"));

        CreateDirs(Path.Combine(resourceRoot, "library", "tool", $"graphics_{newCustomerName}"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "library", "tool", "metric"),
            Path.Combine(resourceRoot, "library", "tool", $"metric_{newCustomerName}"));

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

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "configuration"),
            Path.Combine(resourceRoot, "configuration"));

        CreateCamConfigFiles(resourceRoot, newCustomerName);

        string auxiliaryMetric = Path.Combine(machRoot, "auxiliary", "tagging", "metric");
        CreateDirs(auxiliaryMetric);

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "auxiliary", "tagging", "metric", "tagging.dat"),
            Path.Combine(auxiliaryMetric, "tagging.dat"));

        CopyFileIfMissing(
            Path.Combine(ugiiBaseDir, "MACH", "auxiliary", "tagging", "metric", "tagging_fea.dat"),
            Path.Combine(auxiliaryMetric, "tagging_fea.dat"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(ugiiBaseDir, "MACH", "resource", "template_dir"),
            Path.Combine(resourceRoot, $"template_dir_{newCustomerName}"));

        CopyDirectoryIfMissingOrEmpty(
            Path.Combine(TemplateRootDir, "Vorlage", "duh_tools"),
            Path.Combine(envRoot, "duh_tools"));

        CopyFileIfMissing(
            Path.Combine(TemplateRootDir, "Vorlage", "custom_dirs.dat"),
            Path.Combine(envRoot, "UGII", "menus", "custom_dirs.dat"));

        CopyFileIfMissing(
            Path.Combine(TemplateRootDir, "Vorlage", "ugii_env.dat"),
            Path.Combine(envRoot, "UGII", "ugii_env.dat"));

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

        CopyFileIfMissing(
            Path.Combine(TemplateRootDir, "Vorlage", "start_apps", "custom_nx.bat"),
            Path.Combine(envRoot, "start_apps", $"custom_nx_{newCustomerName}.bat"));
    }

    private void CreateCamConfigFiles(string resourceRoot, string customer)
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

    private void CreateCamConfigFile(
        string sourceFile,
        string targetFile,
        string customer,
        string suffix)
    {
        if (!File.Exists(sourceFile))
            throw new FileNotFoundException($"Die Quelldatei '{sourceFile}' wurde nicht gefunden.", sourceFile);

        if (File.Exists(targetFile))
            return;

        var headerLines = new[]
        {
            $"TEMPLATE_OPERATION,${{UGII_CAM_TEMPLATE_SET_DIR}}cam_{customer}{suffix}.opt",
            $"TEMPLATE_DOCUMENTATION,${{UGII_CAM_SHOP_DOC_DIR}}shop_doc_{customer}.dat",
            $"TEMPLATE_POST,${{UGII_CAM_POST_DIR}}template_post_{customer}.dat"
        };

        var sourceLinesFromLine4 = File.ReadAllLines(sourceFile).Skip(3);

        WriteAllLinesTracked(targetFile, headerLines.Concat(sourceLinesFromLine4));
    }

    private void WriteAllLinesTracked(string file, IEnumerable<string> contents)
    {
        BackupFileIfExists(file);
        CreateDirTracked(Path.GetDirectoryName(file)!);
        File.WriteAllLines(file, contents);

        if (!fileBackups.ContainsKey(file))
            TrackCreatedFile(file);
    }

    private void CreateDirs(params string[] directories)
    {
        foreach (string dir in directories)
        {
            if (!string.IsNullOrWhiteSpace(dir))
                CreateDirTracked(dir);
        }
    }

    private void CopyDirectoryIfMissingOrEmpty(
        string sourceDirectory,
        string targetDirectory,
        string? existenceCheckPath = null)
    {
        string checkPath = existenceCheckPath ?? targetDirectory;

        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException($"Das Quellverzeichnis '{sourceDirectory}' wurde nicht gefunden.");

        if (Directory.Exists(checkPath))
            return;

        CopyDirectory(sourceDirectory, targetDirectory, overwriteFiles: false);
    }

    private void CopyDirectory(string sourceDirectory, string targetDirectory, bool overwriteFiles)
    {
        CreateDirTracked(targetDirectory);

        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string targetSubDirectory = Path.Combine(targetDirectory, Path.GetFileName(directory));
            CopyDirectory(directory, targetSubDirectory, overwriteFiles);
        }

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string targetFile = Path.Combine(targetDirectory, Path.GetFileName(file));
            CopyFileTracked(file, targetFile, overwriteFiles);
        }
    }
}
