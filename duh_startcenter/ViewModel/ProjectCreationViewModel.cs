using NXStartCenter.Services;
using NXStartCenter.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace NXStartCenter.ViewModel
{
    public sealed class ProjectCreationViewModel : BaseViewModel
    {
        private string _newCustomer = string.Empty;
        private string _newVersion = string.Empty;
        private string _newMachine = string.Empty;
        private string _newOrderNumber = "0000";
        private string _selectedMachineType = "Mill machine";
        private string _selectedController = "Sinumerik";
        private bool _complexEnvRequired = true;
        private NewProjectService _projectService;
        private AppModel _model;

        public ICommand CreateProjectCommand { get; }

        private readonly Func<Func<string>, string, Task> _runActionAsync;
        private readonly Action _saveLastSelection;
        private readonly Action _refreshCollectionsFromModel;

        public IReadOnlyList<string> MachineTypes { get; }
        public IReadOnlyList<string> MachineControllers { get; }

        public ProjectCreationViewModel(
                AppModel model,
                NewProjectService projectService,
                Func<Func<string>, string, Task> runActionAsync,
                Action saveLastSelection,
                Action refreshCollectionsFromModel)
        {
            _model = model;
            _projectService = projectService;
            _runActionAsync = runActionAsync;
            _saveLastSelection = saveLastSelection;
            _refreshCollectionsFromModel = refreshCollectionsFromModel;
            _projectService = new NewProjectService(model);
            CreateProjectCommand = new RelayCommand(CreateProject);

            MachineTypes = _model.MachineTypes.Keys.ToArray();
            MachineControllers = _model.MachineControllers;
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

        public bool ComplexEnvRequired
        {
            get => _complexEnvRequired;
            set => SetField(ref _complexEnvRequired, value);
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

        private void CreateProject()
        {
            _runActionAsync(() =>
            {
                var message = _projectService.CreateOrExtendCustomerEnvironment(
                    NewCustomer,
                    NewVersion,
                    NewMachine,
                    NewOrderNumber,
                    SelectedMachineType,
                    SelectedController,
                    ComplexEnvRequired);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _model.Customer = NewCustomer;
                    _model.VersionName = NewVersion;
                    _model.Machine = NewMachine;
                    _saveLastSelection();
                    _model.RefreshAll();
                    _refreshCollectionsFromModel();
                });

                return message;
            },
            "Erstelle neue Kundenumgebung...");
        }
    }
}
