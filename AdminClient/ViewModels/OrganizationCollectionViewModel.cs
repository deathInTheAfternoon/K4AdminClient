using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdminClient.ViewModels
{
    public partial class OrganizationCollectionViewModel : BaseCollectionViewModel<Organization>
    {
        private readonly string _regionId;
        // The selected organization
        public event EventHandler<Organization> OrganizationSelected;

        public OrganizationCollectionViewModel(ApiService apiService, string regionId = "us")
            : base(apiService)
        {
            _regionId = regionId;
            CollectionTitle = "Organizations";
            LoadDataAsync().ConfigureAwait(false);
        }

        protected override async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var organizations = await _apiService.GetOrganizationsForRegionAsync(_regionId);
                Items.Clear();
                foreach (var org in organizations)
                {
                    Items.Add(org);
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

        protected override async Task AddAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                // Create and setup dialog
                var dialogViewModel = new CreateOrganizationViewModel(_apiService, _regionId);
                var dialog = new CreateOrganizationDialog { DataContext = dialogViewModel };

                dialogViewModel.OrganizationCreated += (s, newOrg) =>
                {
                    Items.Add(newOrg);
                };

                // Show dialog and wait for result
                await MaterialDesignThemes.Wpf.DialogHost.Show(dialog);
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
    }
}