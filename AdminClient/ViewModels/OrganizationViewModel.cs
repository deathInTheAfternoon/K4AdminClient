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

        public event EventHandler<Organization> OrganizationSelected;

        public OrganizationViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoadOrganizationsAsync().ConfigureAwait(false);
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
        private async Task CreateOrganizationAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var name = $"New Organization {Organizations.Count + 1}";
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
        private void DrillDown()
        {
            if (SelectedOrganization != null)
            {
                OrganizationSelected?.Invoke(this, SelectedOrganization);
            }
        }
    }
}