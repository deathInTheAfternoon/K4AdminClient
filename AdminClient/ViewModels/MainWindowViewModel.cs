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
        private object _currentView;

        // Reference to the API service that we can pass to other ViewModels
        private readonly ApiService _apiService;

        public MainWindowViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // When the application starts, we could automatically navigate to a default view
            // For example, navigate to the Organizations view:
            NavigateToOrganizations();
        }

        [RelayCommand]
        private void Navigate(Type viewType)
        {
            switch (viewType)
            {
                case Type t when t == typeof(OrganizationView):
                    NavigateToOrganizations();
                    break;
                case Type t when t == typeof(ProgramView):
                    NavigateToProgram();
                    break;
            }
        }


        // Command to navigate to Organizations view
        [RelayCommand]
        private void NavigateToOrganizations()
        {
            CurrentViewTitle = "Organizations";
            // Create a new OrganizationViewModel using our ApiService
            CurrentView = new OrganizationViewModel(_apiService);
        }

        // We can add more navigation commands as needed
        [RelayCommand]
        private void NavigateToProgram()
        {
            CurrentViewTitle = $"Programs";
            CurrentView = new ProgramViewModel(_apiService);
        }
    }
}