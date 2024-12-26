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
    public partial class OperatingUnitDetailViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly OperatingUnit _operatingUnit;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private Program _program;

        public OperatingUnitDetailViewModel(ApiService apiService, OperatingUnit operatingUnit)
        {
            _apiService = apiService;
            _operatingUnit = operatingUnit;
            Name = operatingUnit.Name;
            Program = operatingUnit.Program;
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var ouDetails = await _apiService.GetOperatingUnitAsync(_operatingUnit.Id);
                // Update additional fields when available

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading operating unit details: {ex.Message}";
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

                _operatingUnit.Name = Name;
                // TODO: Add API endpoint for updating
                //await _apiService.UpdateOperatingUnitAsync(_operatingUnit);

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving operating unit: {ex.Message}";
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
