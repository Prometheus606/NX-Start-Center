using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using NXStartCenter.Configuration;
using NXStartCenter.Services;

namespace NXStartCenter;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly AppModel _model;
    private readonly NxService _nxService;
    private readonly ProjectService _projectService;

    private string _statusText = "Bereit";
    private string _newCustomer = string.Empty;
    private string _newVersion = string.Empty;
    private string _newMachine = string.Empty;
    private string _newOrderNumber = "0000";
    private string _selectedMachineType = "Mill machine";
    private string _selectedController = "Sinumerik";

    public MainViewModel()
    {
        var configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NXStartCenter",
            "config.json");

        _model = AppModel.Load(configPath);
        _nxService = new NxService(_model);
        _projectService = new ProjectService(_model);

        Customers = new ObservableCollection<string>();
        Versions = new ObservableCollection<string>();
        Machines = new ObservableCollection<string>();
        NativeVersions = new ObservableCollection<string>();
        PostbuilderVersions = new ObservableCollection<string>();

        Teams = ["CAM", "PP"];
        Themes = ["cosmo", "flatly", "minty", "pulse", "lumen", "solar", "darkly", "cyborg"];
        Editors = ["Notepad", "Notepad++", "VSCode"];
        Languages = ["german", "english"];
        MachineTypes = _model.MachineTypes.Keys.ToArray();
        MachineControllers = _model.MachineControllers;

        StartNxNativeCommand = new RelayCommand(() => RunAction(_nxService.StartNativeNx));
        StartCustomerNxCommand = new RelayCommand(() => RunAction(_nxService.StartCustomerNx));
        StartPostbuilderCommand = new RelayCommand(() => RunAction(_nxService.StartPostbuilder));
        OpenExplorerCommand = new RelayCommand(() => RunAction(_nxService.OpenMachineFolder));
        OpenVsCodeCommand = new RelayCommand(() => RunAction(_nxService.OpenVsCode));
        OpenForkCommand = new RelayCommand(() => RunAction(_nxService.OpenFork));
        OpenMainBatch = new RelayCommand(() => RunAction(_nxService.OpenMainBatch));
        OpenCustomerBatch = new RelayCommand(() => RunAction(_nxService.OpenCustomerBatch));
        OpenDeveloperBatch = new RelayCommand(() => RunAction(_nxService.OpenDeveloperBatch));
        OpenLicenseFileCommand = new RelayCommand(() => RunAction(_nxService.OpenLicenseFile));
        StartLmToolsCommand = new RelayCommand(() => RunAction(_nxService.StartLmTools));
        SaveSettingsCommand = new RelayCommand(SaveSettings);
        RefreshCommand = new RelayCommand(RefreshAll);
        CreateProjectCommand = new RelayCommand(CreateProject);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        ShowInfoCommand = new RelayCommand(ShowInfo);

    RefreshCollectionsFromModel();
    }

    public AppSettings Settings => _model.Settings;
    public AppInfo AppInfo => _model.AppInfo;

    public string HeaderInfo => $"{AppMetadata.Version} | {AppMetadata.AppDate}";
    public string CurrentUser => Environment.UserDomainName + "\\" + Environment.UserName;
    public string CurrentProjectPath => string.IsNullOrWhiteSpace(SelectedMachine) ? string.Empty : Path.Combine(_model.GetInstalledMachinesPath(), SelectedMachine);
    public string ConfigPath => _model.ConfigPath;

    public ObservableCollection<string> Customers { get; }
    public ObservableCollection<string> Versions { get; }
    public ObservableCollection<string> Machines { get; }
    public ObservableCollection<string> NativeVersions { get; }
    public ObservableCollection<string> PostbuilderVersions { get; }

    public IReadOnlyList<string> Teams { get; }
    public IReadOnlyList<string> Themes { get; }
    public IReadOnlyList<string> Editors { get; }
    public IReadOnlyList<string> Languages { get; }
    public IReadOnlyList<string> MachineTypes { get; }
    public IReadOnlyList<string> MachineControllers { get; }

    public ICommand StartNxNativeCommand { get; }
    public ICommand StartCustomerNxCommand { get; }
    public ICommand StartPostbuilderCommand { get; }
    public ICommand OpenExplorerCommand { get; }
    public ICommand OpenVsCodeCommand { get; }
    public ICommand OpenForkCommand { get; }
    public ICommand OpenLicenseFileCommand { get; }
    public ICommand StartLmToolsCommand { get; }
    public ICommand SaveSettingsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand CreateProjectCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand OpenMainBatch { get; }
    public ICommand OpenCustomerBatch { get; }
    public ICommand OpenDeveloperBatch { get; }
    public ICommand ShowInfoCommand { get; }

    public string StatusText
    {
        get => _statusText;
        private set => SetField(ref _statusText, value);
    }

    public string SelectedCustomer
    {
        get => _model.Customer;
        set
        {
            if (_model.Customer == value) return;
            _model.Customer = value ?? string.Empty;
            _model.RefreshVersions();
            RefreshVersionsAndMachinesFromModel();
            SaveLastSelection();
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentProjectPath));
        }
    }

    public string SelectedVersion
    {
        get => _model.VersionName;
        set
        {
            if (_model.VersionName == value) return;
            _model.VersionName = value ?? string.Empty;
            _model.RefreshMachines();
            RefreshMachinesFromModel();
            SaveLastSelection();
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentProjectPath));
        }
    }

    public string SelectedMachine
    {
        get => _model.Machine;
        set
        {
            if (_model.Machine == value) return;
            _model.Machine = value ?? string.Empty;
            SaveLastSelection();
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentProjectPath));
        }
    }

    public string SelectedNativeVersion
    {
        get => _model.NativeVersion;
        set
        {
            if (_model.NativeVersion == value) return;
            _model.NativeVersion = value ?? string.Empty;
            _model.Last.LastNativeVersion = _model.NativeVersion;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public string SelectedPostbuilderVersion
    {
        get => _model.PostbuilderVersion;
        set
        {
            if (_model.PostbuilderVersion == value) return;
            _model.PostbuilderVersion = value ?? string.Empty;
            _model.Last.LastPostbuilderVersion = _model.PostbuilderVersion;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public string SelectedLanguage
    {
        get => _model.Last.LastLanguage;
        set
        {
            if (_model.Last.LastLanguage == value) return;
            _model.Last.LastLanguage = string.IsNullOrWhiteSpace(value) ? "german" : value;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadPp
    {
        get => _model.Last.LastLoadPp != 0;
        set
        {
            var intValue = value ? 1 : 0;
            if (_model.Last.LastLoadPp == intValue) return;
            _model.Last.LastLoadPp = intValue;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadInstalledMachines
    {
        get => _model.Last.LastLoadInstalledMachines != 0;
        set
        {
            var intValue = value ? 1 : 0;
            if (_model.Last.LastLoadInstalledMachines == intValue) return;
            _model.Last.LastLoadInstalledMachines = intValue;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadTool
    {
        get => _model.Last.LastLoadTool != 0;
        set
        {
            var intValue = value ? 1 : 0;
            if (_model.Last.LastLoadTool == intValue) return;
            _model.Last.LastLoadTool = intValue;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadDevice
    {
        get => _model.Last.LastLoadDevice != 0;
        set
        {
            var intValue = value ? 1 : 0;
            if (_model.Last.LastLoadDevice == intValue) return;
            _model.Last.LastLoadDevice = intValue;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public bool LoadFeed
    {
        get => _model.Last.LastLoadFeed != 0;
        set
        {
            var intValue = value ? 1 : 0;
            if (_model.Last.LastLoadFeed == intValue) return;
            _model.Last.LastLoadFeed = intValue;
            _model.Save();
            OnPropertyChanged();
        }
    }

    public string NewCustomer
    {
        get => _newCustomer;
        set => SetField(ref _newCustomer, value ?? string.Empty);
    }

    public string NewVersion
    {
        get => _newVersion;
        set => SetField(ref _newVersion, value ?? string.Empty);
    }

    public string NewMachine
    {
        get => _newMachine;
        set => SetField(ref _newMachine, value ?? string.Empty);
    }

    public string NewOrderNumber
    {
        get => _newOrderNumber;
        set => SetField(ref _newOrderNumber, value ?? string.Empty);
    }

    public string SelectedMachineType
    {
        get => _selectedMachineType;
        set => SetField(ref _selectedMachineType, value ?? "Mill machine");
    }

    public string SelectedController
    {
        get => _selectedController;
        set => SetField(ref _selectedController, value ?? "Sinumerik");
    }

    private void RefreshAll()
    {
        _model.RefreshAll();
        RefreshCollectionsFromModel();
        StatusText = "Daten aktualisiert.";
    }

    private void SaveSettings()
    {
        _model.Save();
        _model.RefreshAll();
        RefreshCollectionsFromModel();
        StatusText = "Einstellungen gespeichert.";
    }

    private void CreateProject()
    {
        RunAction(() =>
        {
            var message = _projectService.CreateCustomerProject(
                NewCustomer,
                NewVersion,
                NewMachine,
                NewOrderNumber,
                SelectedMachineType,
                SelectedController);

            _model.Customer = NewCustomer;
            _model.VersionName = NewVersion;
            _model.Machine = NewMachine;
            SaveLastSelection();
            _model.RefreshAll();
            RefreshCollectionsFromModel();
            return message;
        });
    }

    private void SaveLastSelection()
    {
        _model.Last.LastCustomer = _model.Customer;
        _model.Last.LastVersion = _model.VersionName;
        _model.Last.LastMachine = _model.Machine;
        _model.Save();
    }

    private void RunAction(Func<string> action)
    {
        try
        {
            var message = action();
            StatusText = message;

            if (message.Contains("nicht", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Fehler", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Achtung", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(message, "NX Start Center", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            OnPropertyChanged(nameof(CurrentProjectPath));
        }
        catch (Exception ex)
        {
            StatusText = "Fehler: " + ex.Message;
            MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenSettings()
    {
        var window = new SettingsWindow
        {
            DataContext = this,
            Owner = Application.Current.MainWindow
        };

        window.ShowDialog();
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
