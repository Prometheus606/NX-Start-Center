using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace NXStartCenter;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly ConfigService _configService = new();

    private string _selectedCustomer = "";
    private string _selectedVersion = "";
    private string _selectedMachine = "";
    private string _newCustomer = "";
    private string _newVersion = "";
    private string _newMachine = "";
    private string _newOrderNumber = "0000";
    private string _currentProjectPath = "";
    private string _statusText = "Bereit";

    public MainViewModel()
    {
        Settings = _configService.Config.Settings;

        Customers = new ObservableCollection<string>();
        Versions = new ObservableCollection<string>();
        Machines = new ObservableCollection<string>();
        NativeVersions = new ObservableCollection<string>();

        Teams = ["CAM", "PP"];
        Themes = ["cosmo", "flatly", "minty", "pulse", "lumen", "solar", "darkly", "cyborg"];
        Editors = ["Notepad", "Notepad++", "VSCode"];

        RefreshCustomersCommand = new RelayCommand(RefreshCustomers);
        SaveSettingsCommand = new RelayCommand(SaveSettings);
        CreateProjectCommand = new RelayCommand(CreateProject);
        StartCustomerNxCommand = new RelayCommand(StartCustomerNx);

        RefreshAll();

        SelectedCustomer = PickLast(Customers, _configService.Config.LastConfiguration.LastCustomer);
        SelectedVersion = PickLast(Versions, _configService.Config.LastConfiguration.LastVersion);
        SelectedMachine = PickLast(Machines, _configService.Config.LastConfiguration.LastMachine);

        NewCustomer = SelectedCustomer;
        NewVersion = PickLast(NativeVersions, _configService.Config.LastConfiguration.LastNativeVersion);
    }

    public AppSettings Settings { get; }

    public ObservableCollection<string> Customers { get; }
    public ObservableCollection<string> Versions { get; }
    public ObservableCollection<string> Machines { get; }
    public ObservableCollection<string> NativeVersions { get; }

    public IReadOnlyList<string> Teams { get; }
    public IReadOnlyList<string> Themes { get; }
    public IReadOnlyList<string> Editors { get; }

    public ICommand RefreshCustomersCommand { get; }
    public ICommand SaveSettingsCommand { get; }
    public ICommand CreateProjectCommand { get; }
    public ICommand StartCustomerNxCommand { get; }

    public string SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (!SetField(ref _selectedCustomer, value))
                return;

            RefreshVersions();
            SelectedVersion = PickLast(Versions, _configService.Config.LastConfiguration.LastVersion);
            SaveLastSelection();
        }
    }

    public string SelectedVersion
    {
        get => _selectedVersion;
        set
        {
            if (!SetField(ref _selectedVersion, value))
                return;

            RefreshMachines();
            SelectedMachine = PickLast(Machines, _configService.Config.LastConfiguration.LastMachine);
            SaveLastSelection();
        }
    }

    public string SelectedMachine
    {
        get => _selectedMachine;
        set
        {
            if (!SetField(ref _selectedMachine, value))
                return;

            CurrentProjectPath = GetMachinePath();
            SaveLastSelection();
        }
    }

    public string NewCustomer
    {
        get => _newCustomer;
        set => SetField(ref _newCustomer, value);
    }

    public string NewVersion
    {
        get => _newVersion;
        set => SetField(ref _newVersion, value);
    }

    public string NewMachine
    {
        get => _newMachine;
        set => SetField(ref _newMachine, value);
    }

    public string NewOrderNumber
    {
        get => _newOrderNumber;
        set => SetField(ref _newOrderNumber, value);
    }

    public string CurrentProjectPath
    {
        get => _currentProjectPath;
        private set => SetField(ref _currentProjectPath, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetField(ref _statusText, value);
    }

    private void RefreshAll()
    {
        RefreshNativeVersions();
        RefreshCustomers();
    }

    private void RefreshCustomers()
    {
        ReplaceCollection(Customers, GetDirectories(Settings.CustomerEnvironmentPath));
        StatusText = $"Kunden geladen: {Customers.Count}";
    }

    private void RefreshVersions()
    {
        Versions.Clear();

        if (string.IsNullOrWhiteSpace(SelectedCustomer))
            return;

        var path = Path.Combine(
            Settings.CustomerEnvironmentPath,
            SelectedCustomer,
            "5_Umgebung");

        ReplaceCollection(Versions, GetNxDirectories(path));
    }

    private void RefreshMachines()
    {
        Machines.Clear();

        if (string.IsNullOrWhiteSpace(SelectedCustomer) ||
            string.IsNullOrWhiteSpace(SelectedVersion))
            return;

        ReplaceCollection(Machines, GetDirectories(GetInstalledMachinesPath()));
    }

    private void RefreshNativeVersions()
    {
        ReplaceCollection(NativeVersions, GetNxDirectories(Settings.NxInstallationPath));
    }

    private string GetInstalledMachinesPath()
    {
        var basePath = Path.Combine(
            Settings.CustomerEnvironmentPath,
            SelectedCustomer,
            "5_Umgebung",
            SelectedVersion,
            "MACH",
            "resource",
            "library",
            "machine",
            "installed_machines");

        var customerSpecificPath = basePath + "_" + SelectedCustomer;

        return Directory.Exists(customerSpecificPath)
            ? customerSpecificPath
            : basePath;
    }

    private string GetMachinePath()
    {
        if (string.IsNullOrWhiteSpace(SelectedMachine))
            return "";

        return Path.Combine(GetInstalledMachinesPath(), SelectedMachine);
    }

    private void SaveSettings()
    {
        _configService.Save();

        RefreshAll();

        StatusText = "Einstellungen gespeichert.";
    }

    private void SaveLastSelection()
    {
        if (string.IsNullOrWhiteSpace(SelectedCustomer))
            return;

        _configService.SaveLastSelection(
            SelectedCustomer,
            SelectedVersion,
            SelectedMachine);
    }

    private void CreateProject()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NewCustomer))
                throw new InvalidOperationException("Bitte Kunde auswählen oder eingeben.");

            if (string.IsNullOrWhiteSpace(NewVersion))
                throw new InvalidOperationException("Bitte NX-Version auswählen.");

            if (string.IsNullOrWhiteSpace(NewMachine))
                throw new InvalidOperationException("Bitte Maschine eingeben.");

            var customerPath = Path.Combine(Settings.CustomerEnvironmentPath, NewCustomer);
            var versionPath = Path.Combine(customerPath, "5_Umgebung", NewVersion);
            var machinePath = Path.Combine(
                versionPath,
                "MACH",
                "resource",
                "library",
                "machine",
                "installed_machines",
                NewMachine);

            Directory.CreateDirectory(machinePath);

            SelectedCustomer = NewCustomer;
            SelectedVersion = NewVersion;
            SelectedMachine = NewMachine;

            RefreshAll();

            StatusText = $"Projekt erstellt: {machinePath}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void StartCustomerNx()
    {
        if (string.IsNullOrWhiteSpace(SelectedCustomer) ||
            string.IsNullOrWhiteSpace(SelectedVersion) ||
            string.IsNullOrWhiteSpace(SelectedMachine))
        {
            MessageBox.Show("Bitte Kunde, Version und Maschine auswählen.");
            return;
        }

        SaveLastSelection();

        StatusText = $"Starte NX für {SelectedCustomer} / {SelectedVersion} / {SelectedMachine}";

        // Hier kommt später deine start_routine.bat-Logik rein.
    }

    private static List<string> GetDirectories(string path)
    {
        if (!Directory.Exists(path))
            return [];

        return Directory
            .GetDirectories(path)
            .Select(Path.GetFileName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .OrderBy(x => x)
            .ToList();
    }

    private static List<string> GetNxDirectories(string path)
    {
        if (!Directory.Exists(path))
            return [];

        return Directory
            .GetDirectories(path)
            .Select(Path.GetFileName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .Where(x => x.StartsWith("NX", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x)
            .ToList();
    }

    private static void ReplaceCollection<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();

        foreach (var item in source)
            target.Add(item);
    }

    private static string PickLast(ObservableCollection<string> values, string last)
    {
        if (!string.IsNullOrWhiteSpace(last) && values.Contains(last))
            return last;

        return values.FirstOrDefault() ?? "";
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