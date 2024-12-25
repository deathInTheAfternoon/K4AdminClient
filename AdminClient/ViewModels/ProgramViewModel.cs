using System.Collections.ObjectModel;
using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    public partial class ProgramViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private Models.Program _program;

        [ObservableProperty]
        private ObservableCollection<OperatingUnit> _operatingUnits = new();

        [ObservableProperty]
        private OperatingUnit _selectedOperatingUnit;

        [ObservableProperty]
        private ObservableCollection<BundleDefinition> _bundles = new();

        public ProgramViewModel(ApiService apiService)
        {
            _apiService = apiService;
            //Program = program;
            LoadDataAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                if (Program == null)
                {
                    ErrorMessage = "Program is not initialized.";
                    return;
                }

                // If we don't have a valid Program.Id but do have an Organization.Id,
                // we need to either load an existing program or create a new one
                if (Program?.Organization?.Id > 0 && Program?.Id == 0)
                {
                    // First try to load existing programs
                    var programs = await _apiService.GetProgramsForOrganizationAsync(Program.Organization.Id);
                    var existingProgram = programs.FirstOrDefault();

                    if (existingProgram != null)
                    {
                        Program = existingProgram;
                    }
                    else
                    {
                        // Create a new program if none exist
                        var newProgram = new Program
                        {
                            Name = $"{Program.Organization.Name} Program",
                            Organization = Program.Organization
                        };
                        Program = await _apiService.CreateProgramAsync(Program.Organization.Id, newProgram);
                    }
                }

                // Now load operating units (using the valid Program.Id)
                var units = await _apiService.GetOperatingUnitsForProgramAsync(Program.Id);
                OperatingUnits.Clear();
                foreach (var unit in units)
                {
                    OperatingUnits.Add(unit);
                }

                // Load bundles
                var programBundles = await _apiService.GetBundleDefinitionsForProgramAsync(Program.Id);
                Bundles.Clear();
                foreach (var bundle in programBundles)
                {
                    Bundles.Add(bundle);
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
        private async Task EditProgram()
        {
            // TODO: Implement edit functionality
        }
    }
}