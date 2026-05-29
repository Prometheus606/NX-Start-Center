using NXStartCenter.Model;
using NXStartCenter.Services;
using NXStartCenter.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NXStartCenter.ViewModel
{
    public sealed class SettingsViewModel : BaseViewModel
    {
        private readonly AppModel _model;
        private readonly GeneralService _generalService;
        private readonly StatusViewModel _status;
        private readonly Action _afterSettingsChanged;
        private AppSettings? _settingsBackup;

        public AppSettings Settings => _model.Settings;

        public IReadOnlyList<string> Teams { get; }
        public IReadOnlyList<string> Editors { get; }

        public ICommand SaveSettingsCommand { get; }
        public ICommand DiscardSettingsCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand BrowseNxInstallationPathCommand { get; }
        public ICommand BrowseCustomerEnvironmentCommand { get; }
        public ICommand BrowseTemplateRootCommand { get; }
        public ICommand BrowseLicencePathCommand { get; }
        public ICommand BrowseLicenceServerPathCommand { get; }
        public ICommand BrowseRolesPathCommand { get; }
        public ICommand BrowseTcPathCommand { get; }

        public SettingsViewModel(
        AppModel model,
        GeneralService generalService,
        StatusViewModel status,
        Action afterSettingsChanged)
        {
            _model = model;
            _generalService = generalService;
            _status = status;
            _afterSettingsChanged = afterSettingsChanged;

            Teams = ["CAM", "PP"];
            Editors = ["Notepad", "Notepad++", "VSCode"];

            SaveSettingsCommand = new RelayCommand(SaveSettings);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            DiscardSettingsCommand = new RelayCommand(DiscardSettings);
            BrowseNxInstallationPathCommand = new RelayCommand(BrowseNxInstallationPath);
            BrowseCustomerEnvironmentCommand = new RelayCommand(BrowseCustomerEnvironment);
            BrowseTemplateRootCommand = new RelayCommand(BrowseTemplateRoot);
            BrowseLicencePathCommand = new RelayCommand(BrowseLicencePath);
            BrowseLicenceServerPathCommand = new RelayCommand(BrowseLicenceServerPath);
            BrowseRolesPathCommand = new RelayCommand(BrowseRolesPath);
            BrowseTcPathCommand = new RelayCommand(BrowseTcPath);
        }

        private void OpenSettings()
        {
            _settingsBackup = new AppSettings
            {
                NxInstallationPath = Settings.NxInstallationPath,
                CustomerEnvironmentPath = Settings.CustomerEnvironmentPath,
                TemplateRoot = Settings.TemplateRoot,
                TcPath = Settings.TcPath,
                LicencePath = Settings.LicencePath,
                LicenceServerPath = Settings.LicenceServerPath,
                RolesPath = Settings.RolesPath,
                Team = Settings.Team,
                Editor = Settings.Editor,
                StartNxWithDebug = Settings.StartNxWithDebug,
                OpenVsCodeWithFork = Settings.OpenVsCodeWithFork,
                ShowPullReminder = Settings.ShowPullReminder
            };

            var window = new SettingsWindow
            {
                DataContext = this,
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();
        }

        private void SaveSettings()
        {
            _model.Save();
            _model.RefreshAll();
            _afterSettingsChanged();
            OnPropertyChanged(nameof(CamVisibility));
            OnPropertyChanged(nameof(PpVisibility));
            OnPropertyChanged(nameof(CamOrPpVisibility));
            _status.SetSuccess("Einstellungen gespeichert.");
        }

        private void DiscardSettings()
        {
            if (_settingsBackup == null)
                return;

            Settings.NxInstallationPath = _settingsBackup.NxInstallationPath;
            Settings.CustomerEnvironmentPath = _settingsBackup.CustomerEnvironmentPath;
            Settings.TemplateRoot = _settingsBackup.TemplateRoot;
            Settings.TcPath = _settingsBackup.TcPath;
            Settings.LicencePath = _settingsBackup.LicencePath;
            Settings.LicenceServerPath = _settingsBackup.LicenceServerPath;
            Settings.RolesPath = _settingsBackup.RolesPath;
            Settings.Team = _settingsBackup.Team;
            Settings.Editor = _settingsBackup.Editor;
            Settings.StartNxWithDebug = _settingsBackup.StartNxWithDebug;
            Settings.OpenVsCodeWithFork = _settingsBackup.OpenVsCodeWithFork;
            Settings.ShowPullReminder = _settingsBackup.ShowPullReminder;

            OnPropertyChanged(nameof(Settings));

            _status.SetError("Änderungen verworfen.");
        }

        private bool IsTeam(string team)
=> string.Equals(Settings.Team, team, StringComparison.OrdinalIgnoreCase);

        public Visibility CamVisibility
        => IsTeam("CAM") ? Visibility.Visible : Visibility.Collapsed;

        public Visibility PpVisibility
            => IsTeam("PP") ? Visibility.Visible : Visibility.Collapsed;

        public Visibility CamOrPpVisibility
            => IsTeam("CAM") || IsTeam("PP")
                ? Visibility.Visible
                : Visibility.Collapsed;
        private void BrowseNxInstallationPath()
        {
            _model.Settings.NxInstallationPath = _generalService.BrowseForFoldersOrFiles(description: "NX Installationsordner auswählen") ?? _model.Settings.NxInstallationPath;
            OnPropertyChanged(nameof(Settings));
        }

        private void BrowseCustomerEnvironment()
        {
            _model.Settings.CustomerEnvironmentPath = _generalService.BrowseForFoldersOrFiles(description: "Kundenumgebungsordner auswählen") ?? _model.Settings.CustomerEnvironmentPath;
            OnPropertyChanged(nameof(Settings));
        }

        private void BrowseTemplateRoot()
        {
            _model.Settings.TemplateRoot = _generalService.BrowseForFoldersOrFiles(description: "Templates Ordner auswählen (Vorlage und Toolbars)") ?? _model.Settings.TemplateRoot;
            OnPropertyChanged(nameof(Settings));
        }

        private void BrowseTcPath()
        {
            _model.Settings.TcPath = _generalService.BrowseForFoldersOrFiles(type: "file", description: "Portal.bat auswählen", filers: "Batch Dateien (*.bat)|*.bat") ?? _model.Settings.TcPath;
            OnPropertyChanged(nameof(Settings));
        }

        private void BrowseLicencePath()
        {
            _model.Settings.LicencePath = _generalService.BrowseForFoldersOrFiles(type: "file", description: "lizenz Datei auswählen", filers: "Batch Dateien (*.lic)|*.lic|Alle Dateien (*.*)|*.*") ?? _model.Settings.LicencePath;
            OnPropertyChanged(nameof(Settings));
        }

        private void BrowseLicenceServerPath()
        {
            _model.Settings.LicenceServerPath = _generalService.BrowseForFoldersOrFiles(type: "file", description: "LMTools.exe auswählen", filers: "Ausführbare Dateien (*.exe)|*.exe") ?? _model.Settings.LicenceServerPath;
            OnPropertyChanged(nameof(Settings));
        }

        private void BrowseRolesPath()
        {
            _model.Settings.RolesPath = _generalService.BrowseForFoldersOrFiles(type: "file", description: "Rollen Dateien auswählen", filers: "NX Rollen Dateien (*.mtx)|*.mtx", multiSelect: true) ?? _model.Settings.RolesPath;
            OnPropertyChanged(nameof(Settings));
        }
    }
}
