using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace AdminClient.ViewModels
{
    public partial class EditOrganizationViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly Organization _originalOrganization;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _name;

        public event EventHandler<Organization> OrganizationUpdated;
        public event EventHandler DialogClosed;

        public EditOrganizationViewModel(ApiService apiService, Organization organization)
        {
            _apiService = apiService;
            _originalOrganization = organization;

            // Initialize properties
            Name = organization.Name;
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                IsLoading = true;

                // Create updated organization object
                var updatedOrg = new Organization
                {
                    Id = _originalOrganization.Id,
                    Name = Name
                };

                // TODO: Add API endpoint for updating organization
                //await _apiService.UpdateOrganizationAsync(updatedOrg);

                // Notify subscribers that update was successful
                OrganizationUpdated?.Invoke(this, updatedOrg);
                DialogClosed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // TODO: Handle error (could add ErrorMessage property and show in UI)
                System.Windows.MessageBox.Show($"Error updating organization: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}