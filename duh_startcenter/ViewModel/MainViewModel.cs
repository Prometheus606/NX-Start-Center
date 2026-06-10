using Microsoft.Win32;
using NXStartCenter.Configuration;
using NXStartCenter.Services;
using Ookii.Dialogs.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NXStartCenter.Model;
using NXStartCenter.View;

namespace NXStartCenter.ViewModel;

public sealed class MainViewModel : BaseViewModel
{
    private readonly AppModel _model;
    private readonly NxService _nxService;
    private readonly GeneralService _generalService;
    private readonly NewProjectService _projectService;


    public MainViewModel()
    {
        var configPath = Path.Combine(
            AppContext.BaseDirectory,
            "data",
            "config.json");

        _model = AppModel.Load(configPath);
        _nxService = new NxService(_model);
        _generalService = new GeneralService(_model);
        _projectService = new NewProjectService(_model);

        Status = new StatusViewModel();
        SettingsVm = new SettingsViewModel(_model, _generalService, Status, AfterSettingsChanged);
        ProjectCreation = new ProjectCreationViewModel(_model, _projectService, RunActionAsync, SaveLastSelection, RefreshCollectionsFromModel);


        Customers = new ObservableCollection<string>();
        Versions = new ObservableCollection<string>();
        Machines = new ObservableCollection<string>();
        NativeVersions = new ObservableCollection<string>();
        PostbuilderVersions = new ObservableCollection<string>();

        _model.ForkPath = GeneralService.GetForkExePath();

        Languages = ["german", "english"];

        if (_model.Last.LastLanguage != "english" && _model.Last.LastLanguage != "german")
        {
            _model.Last.LastLanguage = "german";
            _model.Save();
        }

        StartNxNativeCommand = new RelayCommand(() => RunAction(_nxService.StartNativeNx));
        StartCustomerNxCommand = new RelayCommand(() => RunAction(_nxService.StartCustomerNx));
        StartPostbuilderCommand = new RelayCommand(() => RunAction(_generalService.StartPostbuilder));
        OpenExplorerCommand = new RelayCommand(() => RunAction(_generalService.OpenMachineFolder));
        OpenVsCodeCommand = new RelayCommand(() => RunAction(_generalService.OpenVsCode));
        OpenVsCodeAndForkCommand = new RelayCommand(() => RunAction(_generalService.OpenVsCodeAndFork));
        OpenForkCommand = new RelayCommand(() => RunAction(_generalService.OpenFork));
        OpenAuxCSECommand = new RelayCommand(() => RunAction(_generalService.OpenAuxCseFolder));
        OpenMainBatch = new RelayCommand(() => RunAction(_generalService.OpenMainBatch));
        OpenCustomerBatch = new RelayCommand(() => RunAction(_generalService.OpenCustomerBatch));
        OpenDeveloperBatch = new RelayCommand(() => RunAction(_generalService.OpenDeveloperBatch));
        OpenLicenseFileCommand = new RelayCommand(() => RunAction(_generalService.OpenLicenseFile));
        StartLmToolsCommand = new RelayCommand(() => RunAction(_generalService.StartLmTools));
        RefreshCommand = new RelayCommand(RefreshAll);
        ShowInfoCommand = new RelayCommand(ShowInfo);

    RefreshCollectionsFromModel();
    }

    public StatusViewModel Status { get; }
    public SettingsViewModel SettingsVm { get; }
    public ProjectCreationViewModel ProjectCreation { get; }

    public AppSettings Settings => _model.Settings;
    public AppInfo AppInfo => _model.AppInfo;

    public string HeaderInfo => $"{AppMetadata.Version} | {AppInfo.AppDate}";
    public string CurrentUser => Environment.UserDomainName + "\\" + Environment.UserName;
    public string CurrentProjectPath => string.IsNullOrWhiteSpace(SelectedMachine) ? string.Empty : Path.Combine(_model.GetInstalledMachinesPath(_model.SelectedCustomer, _model.SelectedVersion), SelectedMachine);
    public string ConfigPath => _model.ConfigPath;

    public ObservableCollection<string> Customers { get; }
    public ObservableCollection<string> Versions { get; }
    public ObservableCollection<string> Machines { get; }
    public ObservableCollection<string> NativeVersions { get; }
    public ObservableCollection<string> PostbuilderVersions { get; }

    public IReadOnlyList<string> Languages { get; }

    public ICommand StartNxNativeCommand { get; }
    public ICommand StartCustomerNxCommand { get; }
    public ICommand StartPostbuilderCommand { get; }
    public ICommand OpenExplorerCommand { get; }
    public ICommand OpenVsCodeCommand { get; }
    public ICommand OpenVsCodeAndForkCommand { get; }
    public ICommand OpenForkCommand { get; }
    public ICommand OpenAuxCSECommand { get; }
    public ICommand OpenLicenseFileCommand { get; }
    public ICommand StartLmToolsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand OpenMainBatch { get; }
    public ICommand OpenCustomerBatch { get; }
    public ICommand OpenDeveloperBatch { get; }
    public ICommand ShowInfoCommand { get; }

    

    public string SelectedCustomer
    {
        get => _model.SelectedCustomer;
        set
        {
            if (_model.SelectedCustomer == value) return;
            _model.SelectedCustomer = value ?? string.Empty;
            _model.RefreshVersions();
            RefreshVersionsAndMachinesFromModel();
            AutoSetLoadOptionsForCustomer();
            SaveLastSelection();
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentProjectPath));
        }
    }

    public bool NxXCheck
    {
        get => _model.StartNxWithCloudLicense;
        set
        {
            if (_model.StartNxWithCloudLicense == value) return;
            _model.StartNxWithCloudLicense = value;


            _model.Last.LastCloudLicenseCheck = _model.StartNxWithCloudLicense;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool TcCheck
    {
        get => _model.StartNxManaged;
        set
        {
            if (_model.StartNxManaged == value) return;
            _model.StartNxManaged = value;
            _model.Last.LastTcCheck = _model.StartNxManaged;
            _model.Save();
            OnPropertyChanged();
        }
    }

    private void AutoSetLoadOptionsForCustomer()
    {
        if (string.IsNullOrWhiteSpace(_model.SelectedCustomer))
            return;

        // Vorerst Beispielpfad
        var customerEnvironmentPath = Path.Combine(
            _model.Settings.CustomerEnvironmentPath,
            _model.SelectedCustomer);

        LoadInstalledMachines = Directory.Exists(
            Path.Combine(customerEnvironmentPath, _model.EnvFolderName, _model.SelectedVersion, "MACH", "resource", "library", "machine", "installed_machines")) || Directory.Exists(
            Path.Combine(customerEnvironmentPath, _model.EnvFolderName, _model.SelectedVersion, "MACH", "resource", "library", "machine", $"installed_machines_{_model.SelectedCustomer}"));

        LoadPp = Directory.Exists(
            Path.Combine(customerEnvironmentPath, _model.EnvFolderName, _model.SelectedVersion, "MACH", "resource", "postprocessor"));

        LoadTool = Directory.Exists(
            Path.Combine(customerEnvironmentPath, _model.EnvFolderName, _model.SelectedVersion, "MACH", "resource", "library", "tool"));

        LoadDevice = Directory.Exists(
            Path.Combine(customerEnvironmentPath, _model.EnvFolderName, _model.SelectedVersion, "MACH", "resource", "library", "device"));

        LoadFeed = Directory.Exists(
            Path.Combine(customerEnvironmentPath, _model.EnvFolderName, _model.SelectedVersion, "MACH", "resource", "library", "feeds_speeds"));

        LoadFullResourceDir = Directory.Exists(
            Path.Combine(customerEnvironmentPath, _model.EnvFolderName, _model.SelectedVersion, "MACH", "resource")) && 
            _generalService.HasFullMachDirContent(Path.Combine(customerEnvironmentPath, _model.EnvFolderName, _model.SelectedVersion, "MACH", "resource"));
    }

    public string SelectedVersion
    {
        get => _model.SelectedVersion;
        set
        {
            if (_model.SelectedVersion == value) return;
            _model.SelectedVersion = value ?? string.Empty;
            _model.RefreshMachines();
            RefreshMachinesFromModel();
            SaveLastSelection();
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentProjectPath));
        }
    }

    public string SelectedMachine
    {
        get => _model.SelectedMachine;
        set
        {
            if (_model.SelectedMachine == value) return;
            _model.SelectedMachine = value ?? string.Empty;
            SaveLastSelection();
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentProjectPath));
        }
    }

    public string SelectedNativeVersion
    {
        get => _model.SelectedNativeVersion;
        set
        {
            if (_model.SelectedNativeVersion == value) return;
            _model.SelectedNativeVersion = value ?? string.Empty;
            _model.Last.LastNativeVersion = _model.SelectedNativeVersion;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public string SelectedPostbuilderVersion
    {
        get => _model.SelectedPostbuilderVersion;
        set
        {
            if (_model.SelectedPostbuilderVersion == value) return;
            _model.SelectedPostbuilderVersion = value ?? string.Empty;
            _model.Last.LastPostbuilderVersion = _model.SelectedPostbuilderVersion;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadPp
    {
        get => _model.Last.LastLoadPp;
        set
        {
            if (_model.Last.LastLoadPp == value) return;
            _model.Last.LastLoadPp = value;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadInstalledMachines
    {
        get => _model.Last.LastLoadInstalledMachines;
        set
        {
            if (_model.Last.LastLoadInstalledMachines == value) return;
            _model.Last.LastLoadInstalledMachines = value;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadTool
    {
        get => _model.Last.LastLoadTool;
        set
        {
            if (_model.Last.LastLoadTool == value) return;
            _model.Last.LastLoadTool = value;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadDevice
    {
        get => _model.Last.LastLoadDevice;
        set
        {
            if (_model.Last.LastLoadDevice == value) return;
            _model.Last.LastLoadDevice = value;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadFeed
    {
        get => _model.Last.LastLoadFeed;
        set
        {
            if (_model.Last.LastLoadFeed == value) return;
            _model.Last.LastLoadFeed = value;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadFullResourceDir
    {
        get => _model.Last.LastLoadFullResourceDir;
        set
        {
            if (_model.Last.LastLoadFullResourceDir == value) return;
            _model.Last.LastLoadFullResourceDir = value;
            _model.Save();
            OnPropertyChanged();
        }
    }

    

    private void RefreshAll()
    {
        _model.RefreshAll();
        RefreshCollectionsFromModel();
        Status.SetSuccess("Daten aktualisiert.");
    }

    private void SaveLastSelection()
    {
        _model.Last.LastCustomer = _model.SelectedCustomer;
        _model.Last.LastVersion = _model.SelectedVersion;
        _model.Last.LastMachine = _model.SelectedMachine;
        _model.Save();
    }

    private void RunAction(Func<string> action, string startMessage = "")
    {
        try
        {
            if (!string.IsNullOrEmpty(startMessage))
            {
                Status.SetBusy(startMessage);
            }

            var message = action();

            if (IsErrorMessage(message))
            {
                Status.SetError(message);
                MessageBox.Show(message, "NX Start Center", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Status.SetSuccess(message);
            OnPropertyChanged(nameof(CurrentProjectPath));
        }
        catch (Exception ex)
        {
            Status.SetError("Fehler: " + ex.Message);
            MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task RunActionAsync(Func<string> action, string startMessage = "")
    {
        try
        {
            if (!string.IsNullOrEmpty(startMessage))
            {
                Status.SetBusy(startMessage);
                await Task.Yield();
            }

            var message = await Task.Run(action);

            if (IsErrorMessage(message))
            {
                Status.SetError(message);
                MessageBox.Show(message, "NX Start Center", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Status.SetSuccess(message);
            OnPropertyChanged(nameof(CurrentProjectPath));
        }
        catch (Exception ex)
        {
            Status.SetError("Fehler: " + ex.Message);
            MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static bool IsErrorMessage(string message)
    {
        return message.Contains("nicht", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("Fehler", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("muss", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("müssen", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("m�ssen", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("keine", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("Achtung", StringComparison.OrdinalIgnoreCase);
    }



    private void ShowInfo()
    {
        var message =
            $"NX Start Center\n\n" +
            $"Version: {AppInfo.Version}\n" +
            $"Datum: {AppInfo.AppDate}\n" +
            $"Autor: {AppInfo.Author}\n" +
            $"Support: {AppInfo.SupportMail}\n";

        MessageBox.Show(
            message,
            "Info",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void RefreshCollectionsFromModel()
    {
        var oldCustomer = SelectedCustomer;
        var oldVersion = SelectedVersion;
        var oldMachine = SelectedMachine;
        var oldNativeVersion = SelectedNativeVersion;
        var oldPostbuilderVersion = SelectedPostbuilderVersion;

        ReplaceCollection(Customers, _model.Customers);
        SelectedCustomer = PickExistingOrFirst(Customers, oldCustomer);

        _model.RefreshVersions();
        ReplaceCollection(Versions, _model.Versions);
        SelectedVersion = PickExistingOrFirst(Versions, oldVersion);

        _model.RefreshMachines();
        ReplaceCollection(Machines, _model.Machines);
        SelectedMachine = PickExistingOrFirst(Machines, oldMachine);

        ReplaceCollection(NativeVersions, _model.NativeVersions);
        SelectedNativeVersion = PickExistingOrFirst(NativeVersions, oldNativeVersion);

        ReplaceCollection(PostbuilderVersions, _model.PostbuilderVersions);
        SelectedPostbuilderVersion = PickExistingOrFirst(PostbuilderVersions, oldPostbuilderVersion);

        OnPropertyChanged(nameof(SelectedCustomer));
        OnPropertyChanged(nameof(SelectedVersion));
        OnPropertyChanged(nameof(SelectedMachine));
        OnPropertyChanged(nameof(SelectedNativeVersion));
        OnPropertyChanged(nameof(SelectedPostbuilderVersion));
        OnPropertyChanged(nameof(SelectedLanguage));
        OnPropertyChanged(nameof(LoadPp));
        OnPropertyChanged(nameof(LoadInstalledMachines));
        OnPropertyChanged(nameof(LoadTool));
        OnPropertyChanged(nameof(LoadDevice));
        OnPropertyChanged(nameof(LoadFeed));
        OnPropertyChanged(nameof(CurrentProjectPath));
    }

    private void RefreshVersionsAndMachinesFromModel()
    {
        var oldVersion = SelectedVersion;
        var oldMachine = SelectedMachine;

        ReplaceCollection(Versions, _model.Versions);
        SelectedVersion = PickExistingOrFirst(Versions, oldVersion);

        _model.RefreshMachines();
        ReplaceCollection(Machines, _model.Machines);
        SelectedMachine = PickExistingOrFirst(Machines, oldMachine);

        OnPropertyChanged(nameof(SelectedVersion));
        OnPropertyChanged(nameof(SelectedMachine));
        OnPropertyChanged(nameof(CurrentProjectPath));
    }

    private void RefreshMachinesFromModel()
    {
        var oldMachine = SelectedMachine;

        ReplaceCollection(Machines, _model.Machines);
        SelectedMachine = PickExistingOrFirst(Machines, oldMachine);

        OnPropertyChanged(nameof(SelectedMachine));
        OnPropertyChanged(nameof(CurrentProjectPath));
    }

    private static void ReplaceCollection<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (var item in source)
            target.Add(item);
    }

    private static string PickExistingOrFirst(
    ObservableCollection<string> values,
    string oldValue)
    {
        if (!string.IsNullOrWhiteSpace(oldValue) && values.Contains(oldValue))
            return oldValue;

        return values.FirstOrDefault() ?? string.Empty;
    }

    public string SelectedLanguage
    {
        get
        {
            var language = _model.Last.LastLanguage;
            return language == "english" ? "english" : "german";
        }
        set
        {
            var language = value == "english" ? "english" : "german";

            if (_model.Last.LastLanguage == language)
                return;

            _model.Last.LastLanguage = language;
            _model.Save();

            OnPropertyChanged();
            OnPropertyChanged(nameof(IsGermanSelected));
            OnPropertyChanged(nameof(IsEnglishSelected));
        }
    }

    public bool IsGermanSelected
    {
        get => SelectedLanguage == "german";
        set
        {
            if (value)
                SelectedLanguage = "german";
        }
    }

    public bool IsEnglishSelected
    {
        get => SelectedLanguage == "english";
        set
        {
            if (value)
                SelectedLanguage = "english";
        }
    }

    private void AfterSettingsChanged()
    {
        RefreshCollectionsFromModel();
        OnPropertyChanged(nameof(Settings));
    }
}
