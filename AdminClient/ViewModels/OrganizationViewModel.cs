using System.Collections.ObjectModel;
using System.ComponentModel;
using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
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
                // Clear any previous error messages
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
                // Always clear loading animation
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
        private void DrillDown()
        {
            if (SelectedOrganization != null)
            {
                // Raise event to notify parent ViewModel
                OrganizationSelected?.Invoke(this, SelectedOrganization);
            }
        }

        [RelayCommand]
        private async Task EditOrganization(Organization organization)
        {
            var dialogViewModel = new EditOrganizationViewModel(_apiService, organization);

            var dialog = new EditOrganizationDialog
            {
                DataContext = dialogViewModel
            };

            dialogViewModel.OrganizationUpdated += (s, updatedOrg) =>
            {
                // Update the organization in the collection
                var index = Organizations.IndexOf(organization);
                if (index != -1)
                {
                    Organizations[index] = updatedOrg;
                }
            };

            // Show dialog
            //var dialogHost = MaterialDesignThemes.Wpf.DialogHost.GetDialogHost("RootDialog");
            var dialogHost = MaterialDesignThemes.Wpf.DialogHost.Show(dialog);
//            await dialogHost.(dialog);
        }
    }
}