﻿// ADD NEW FILE: ViewModels/CreateOrganizationViewModel.cs
using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    public partial class CreateOrganizationViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly string _regionId;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _name;

        // Event emitters
        public event EventHandler<Organization> OrganizationsCollectionUpdated;
        public event EventHandler DialogClosed;

        public CreateOrganizationViewModel(ApiService apiService, string regionId = "us")
        {
            _apiService = apiService;
            _regionId = regionId;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogClosed?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private async Task Create()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                System.Windows.MessageBox.Show("Organization name cannot be empty");
                return;
            }

            try
            {
                IsLoading = true;

                var newOrg = new Organization { Name = Name };
                var createdOrg = await _apiService.CreateOrganizationAsync(_regionId, newOrg);

                // Invoke client handlers
                OrganizationsCollectionUpdated?.Invoke(this, createdOrg);
                DialogClosed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error creating organization: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}