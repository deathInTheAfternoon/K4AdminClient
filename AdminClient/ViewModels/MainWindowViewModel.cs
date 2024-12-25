using System;
using System.Windows.Input;
using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    // The partial keyword is needed for the source generators from CommunityToolkit.Mvvm
    public partial class MainWindowViewModel : ObservableObject
    {
        // Observable property for the current view/page title
        [ObservableProperty]
        private string _currentViewTitle = "Welcome";

        // Observable property that will hold our current page/view
        [ObservableProperty]
        private object _currentViewModel;

        // Reference to the API service that we can pass to other ViewModels
        private readonly ApiService _apiService;

        public MainWindowViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // Set the initial OrganizationViewModel as the default model
            CurrentViewModel = new OrganizationViewModel(_apiService);
            CurrentViewTitle = "Organizations";
        }

        [RelayCommand]
        private void Navigate(Type viewModelType)
        {
            // Create and set the ViewModel
            if (viewModelType == typeof(OrganizationViewModel))
            {
                CurrentViewTitle = "Organizations";
                CurrentViewModel = new OrganizationViewModel(_apiService);
            }
            else if (viewModelType == typeof(ProgramViewModel))
            {
                CurrentViewTitle = "Programs";
                CurrentViewModel = new ProgramViewModel(_apiService);
            }
        }
    }
}