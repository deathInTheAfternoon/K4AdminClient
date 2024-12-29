using System.Collections.ObjectModel;
using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    public partial class ProgramViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private Organization _organization;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private Models.Program _program;

        [ObservableProperty]
        private ObservableCollection<Program> _programs = new();

        [ObservableProperty]
        private Program _selectedProgram;

        [ObservableProperty]
        private ObservableCollection<OperatingUnit> _operatingUnits = new();

        [ObservableProperty]
        private OperatingUnit _selectedOperatingUnit;

        [ObservableProperty]
        private ObservableCollection<BundleDefinition> _bundles = new();

        public ProgramViewModel(ApiService apiService, Organization organization)
        {
            _apiService = apiService;
            _organization = organization;
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                if (Organization == null)
                {
                    ErrorMessage = "Organization reference is missing";
                    return;
                }

                // Load programs for the organization
                var orgPrograms = await _apiService.GetProgramsForOrganizationAsync(Organization.Id);
                Programs.Clear();
                foreach (var prog in orgPrograms)
                {
                    Programs.Add(prog);
                }

                // Set the current program or create initial if none exists
                Program = Programs.FirstOrDefault() ?? await CreateInitialProgramAsync();

                // Load operating units for the selected program
                if (Program != null)
                {
                    var units = await _apiService.GetOperatingUnitsForProgramAsync(Program.Id);
                    OperatingUnits.Clear();
                    foreach (var unit in units)
                    {
                        OperatingUnits.Add(unit);
                    }

                    // Load bundles
                    var bundles = await _apiService.GetBundleDefinitionsForProgramAsync(Program.Id);
                    Bundles.Clear();
                    foreach (var bundle in bundles)
                    {
                        Bundles.Add(bundle);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Persist an empty Program to DB
        /// </summary>
        /// <returns></returns>
        private async Task<Program> CreateInitialProgramAsync()
        {
            var newProgram = new Program
            {
                Name = $"{Organization.Name} Program",
                Organization = Organization
            };
            return await _apiService.CreateProgramAsync(Organization.Id, newProgram);
        }

        [RelayCommand]
        private async Task CreateOperatingUnit()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var newUnit = new OperatingUnit
                {
                    Name = "New Operating Unit",
                    Program = Program
                };

                var createdUnit = await _apiService.CreateOperatingUnitAsync(Program.Id, newUnit);
                OperatingUnits.Add(createdUnit);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating operating unit: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateBundleDefinition()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var newBundleDefinition = new BundleDefinition
                {
                    Name = "New Bundle Definition",
                    Program = Program,
                    Status = BundleStatus.DRAFT
                };

                var createdBundleDefinition = await _apiService.CreateBundleDefinitionAsync(Program.Id, newBundleDefinition);
                Bundles.Add(createdBundleDefinition);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating bundle definition: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateProgram()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var newProgram = new Program
                {
                    Name = $"New Program {Programs.Count + 1}",
                    Organization = Organization
                };

                var createdProgram = await _apiService.CreateProgramAsync(Organization.Id, newProgram);
                Programs.Add(createdProgram);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating program: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task EditProgram(Program program)
        {
            if (program == null) return;

            var dialogViewModel = new EditProgramViewModel(_apiService, program);
            dialogViewModel.ProgramUpdated += (s, updatedProgram) =>
            {
                var index = Programs.IndexOf(program);
                if (index != -1)
                {
                    Programs[index] = updatedProgram;
                }
            };

            var dialog = new EditProgramDialog { DataContext = dialogViewModel };
            await MaterialDesignThemes.Wpf.DialogHost.Show(dialog);
        }
    }
}