using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    public partial class OrganizationDetailViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly Organization _organization;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        public OrganizationDetailViewModel(ApiService apiService, Organization organization)
        {
            _apiService = apiService;
            _organization = organization;
            Name = organization.Name;
            // Add other properties as needed
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                // Load additional organization details if needed
                var orgDetails = await _apiService.GetOrganizationAsync(_organization.Id);
                // Populate additional fields

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading organization details: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                _organization.Name = Name;
                // Update other properties

                // TODO: Add API endpoint for updating
                //await _apiService.UpdateOrganizationAsync(_organization);

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving organization: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            // Handle cancel action
        }
    }
}
