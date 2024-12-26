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
    public partial class OperatingUnitViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private OperatingUnit _operatingUnit;

        // ADD collections for HCPs and Subjects
        [ObservableProperty]
        private ObservableCollection<User> _hcps = new();

        [ObservableProperty]
        private ObservableCollection<User> _subjects = new();

        public OperatingUnitViewModel(ApiService apiService, OperatingUnit operatingUnit)
        {
            _apiService = apiService;
            _operatingUnit = operatingUnit;
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                // Here we would load HCPs and Subjects
                // Note: API endpoint needs to be added to ApiService
                // _apiService.GetHcpsForOperatingUnitAsync(OperatingUnit.Id);
                // _apiService.GetSubjectsForOperatingUnitAsync(OperatingUnit.Id);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading operating unit data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
