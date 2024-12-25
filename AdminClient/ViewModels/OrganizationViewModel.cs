using System.Collections.ObjectModel;
using System.ComponentModel;
using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Program = AdminClient.Models.Program;

namespace AdminClient.ViewModels
{
    // Using source generators from CommunityToolkit.Mvvm for cleaner property notifications
    public partial class OrganizationViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly string _regionId = "us"; // Default region

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private ObservableCollection<Organization> _organizations = new();

        [ObservableProperty]
        private Organization _selectedOrganization;

        [ObservableProperty]
        private ObservableCollection<Program> _programs = new();

        [ObservableProperty]
        private Program _selectedProgram;

        private readonly Action<Program> _onProgramSelected;

        [RelayCommand]
        private void NavigateToProgram()
        {
            if (SelectedProgram != null)
            {
                _onProgramSelected(SelectedProgram);
            }
        }

        public OrganizationViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoadOrganizationsAsync().ConfigureAwait(false);
            //_onProgramSelected = onProgramSelected;


            //if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            //{
            //    LoadSampleData();
            //}
            //else
            //{
            //    // Load real data when constructed
            //    LoadOrganizationsAsync().ConfigureAwait(false);
            //}
        }

        [RelayCommand]
        private async Task LoadOrganizationsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var orgs = await _apiService.GetOrganizationsForRegionAsync(_regionId);
                Organizations.Clear();
                foreach (var org in orgs)
                {
                    Organizations.Add(org);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading organizations: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadProgramsForSelectedOrganizationAsync()
        {
            if (SelectedOrganization == null) return;

            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var progs = await _apiService.GetProgramsForOrganizationAsync(SelectedOrganization.Id);
                Programs.Clear();
                foreach (var prog in progs)
                {
                    Programs.Add(prog);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading programs: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateOrganizationAsync(string name)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var newOrg = new Organization { Name = name };
                var createdOrg = await _apiService.CreateOrganizationAsync(_regionId, newOrg);
                Organizations.Add(createdOrg);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating organization: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateProgramAsync(string name)
        {
            if (SelectedOrganization == null) return;

            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var newProgram = new Program
                {
                    Name = name,
                    Organization = SelectedOrganization
                };

                var createdProgram = await _apiService.CreateProgramAsync(SelectedOrganization.Id, newProgram);
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

        // Helper method to load sample design-time data
        private void LoadSampleData()
        {
            Organizations.Clear();
            Organizations.Add(new Organization { Id = 1, Name = "Sample Organization 1" });
            Organizations.Add(new Organization { Id = 2, Name = "Sample Organization 2" });

            Programs.Clear();
            Programs.Add(new AdminClient.Models.Program { Id = 1, Name = "Sample Program 1" });
            Programs.Add(new AdminClient.Models.Program { Id = 2, Name = "Sample Program 2" });
        }

        // Property changed handlers
        partial void OnSelectedOrganizationChanged(Organization value)
        {
            if (value != null && !DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                LoadProgramsForSelectedOrganizationAsync().ConfigureAwait(false);
            }
        }
    }
}