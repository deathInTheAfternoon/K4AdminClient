using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdminClient.ViewModels
{
    public partial class BundleDefinitionViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private BundleDefinition _bundleDefinition;

        [ObservableProperty]
        private ObservableCollection<ActivityDefinition> _activities = new();

        public BundleDefinitionViewModel(ApiService apiService, BundleDefinition bundleDefinition)
        {
            _apiService = apiService;
            _bundleDefinition = bundleDefinition;
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                // Load activities from the bundle
                foreach (var activity in _bundleDefinition.Activities)
                {
                    Activities.Add(activity);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading bundle definition data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
