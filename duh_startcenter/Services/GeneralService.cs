using Microsoft.Win32;
using NXStartCenter;
using NXStartCenter.Model;
using Ookii.Dialogs.Wpf;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace NXStartCenter.Services;


public sealed class GeneralService(AppModel model)
{

    public static void RemoveFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public string StartPostbuilder()
    {
        if (string.IsNullOrWhiteSpace(model.SelectedPostbuilderVersion)) return "Keine Postbuilder-Version gewählt.";
        var batch = Path.Combine(model.Settings.NxInstallationPath, model.SelectedPostbuilderVersion, "POSTBUILD", "post_builder.bat");
        if (!File.Exists(batch)) return "Postbuilder Version kann nicht gestartet werden!\n" + batch;
        ProcessService.StartFile(batch, $"\"{Path.Combine(model.Settings.NxInstallationPath, model.SelectedPostbuilderVersion)}\\\"");
        model.Last.LastPostbuilderVersion = model.SelectedPostbuilderVersion;
        model.Save();
        return "Postbuilder wurde gestartet.";
    }

    public string OpenMachineFolder()
    {
        var dir = Path.Combine(model.GetInstalledMachinesPath(model.SelectedCustomer, model.SelectedVersion), model.SelectedMachine);
        if (!Directory.Exists(dir)) return "Der Ordner konnte nicht geöffnet werden.\n" + dir;
        ProcessService.OpenFolder(dir);
        return "Ordner geöffnet.";
    }

    public string OpenFork()
    {
        var dir = Path.Combine(model.GetInstalledMachinesPath(model.SelectedCustomer, model.SelectedVersion), model.SelectedMachine);
        if (!File.Exists(model.ForkPath)) return "Fork ist nicht installiert oder der Pfad zur Fork.exe ist falsch.";
        if (!Directory.Exists(Path.Combine(dir, ".git"))) MessageBox.Show("Achtung: Kein Repository angelegt!", "Info");
        if (model.Settings.ShowPullReminder)
        {
            ShowPullReminder();
        }
        
        ProcessService.StartFile(model.ForkPath, $"\"{dir}\"");
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
        var ppDir = Path.Combine(model.GetInstalledMachinesPath(model.SelectedCustomer, model.SelectedVersion), model.SelectedMachine, "postprocessor");
        if (!Directory.Exists(ppDir)) return "Der PP Ordner konnte nicht geöffnet werden da er nicht existiert.\n" + ppDir;
        ProcessService.StartFile(ProcessService.FindEditor(model.Settings.Editor) ?? "code", $"\"{ppDir}\"");
        return "VS Code wurde geöffnet.";
    }

    public string OpenVsCodeAndFork()
    {
        string message = "";
        if (model.Settings.OpenVsCodeWithFork)
        {
            message = OpenFork();
        }
        message = OpenVsCode();
        if (message != "VS Code wurde geöffnet.")
        {
            return message; 
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
        return OpenFile("Batch_files/start_routine.bat");
    }

    public string OpenCustomerBatch()
    {
        return OpenFile(Path.Combine(model.Settings.CustomerEnvironmentPath, model.SelectedCustomer, model.EnvFolderName, model.SelectedVersion, "start_apps", $"custom_nx_{model.SelectedCustomer}.bat"));
    }

    public string OpenDeveloperBatch()
    {
        return OpenFile("Batch_files/user_settings.bat");
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

    public string? BrowseForFoldersOrFiles(string type = "folder", string description = "Pfad Auswählen", string filers = "Alle Dateien (*.*)|*.*", bool multiSelect = false)
    {
        if (type == "folder")
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = description,
                UseDescriptionForTitle = true
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return dialog.SelectedPath;
            }
        }
        else
        {
            var dialog = new OpenFileDialog
            {
                Title = description,
                Filter = filers,
                Multiselect = multiSelect,
                CheckFileExists = true
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return multiSelect
                    ? string.Join(";", dialog.FileNames)
                    : dialog.FileName;
            }
        }
        return null;
    }

    public static string GetForkExePath()
    {
        string path = ProcessService.FindInstallLocation("Fork") ?? "";
        if (!path.EndsWith(@"\current\Fork.exe"))
        {
            path = Path.Combine(path, "current", "Fork.exe");
        }
        return path;
    }

    public bool HasFullMachDirContent(string resourceDirPath)
    {

        if (!Path.Exists(resourceDirPath)) return false;

        string?[] orgFolders = new string[]
        {
            "configuration", "debug",  "feature", "library",
            "machining_knowledge", "owi",  "post_configurator", "postprocessor",
            "probing_cycles", "robots",  "shop_doc", "spreadsheet",
            "template_dir", "template_part",  "template_set", "tool_path",
            "ug_library", "user_def_event",  "wizard"
        };

        string?[] currentFolders = Directory
            .GetDirectories(resourceDirPath)
            .Select(Path.GetFileName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        foreach (string? folderName in orgFolders)
        {
            if (!currentFolders.Contains(folderName, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}
