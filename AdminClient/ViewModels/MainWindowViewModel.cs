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
        // Current ViewModel
        [ObservableProperty]
        private object _currentViewModel;
        // Current view's title
        [ObservableProperty]
        private string _currentViewTitle = "Welcome";

        // Add navigation stack and 'back' command.
        [ObservableProperty]
        private bool _canNavigateBack;
        private readonly Stack<(object ViewModel, string Title)> _navigationStack = new();
        private bool _disposed;

        // Reference to the API service that we will pass down to other ViewModels
        private readonly ApiService _apiService;

        public MainWindowViewModel(ApiService apiService)
        {
            _apiService = apiService;
            var orgViewModel = new OrganizationViewModel(_apiService);
            // Add handler for when an organization is selected
            orgViewModel.OrganizationSelected += OnOrganizationSelected;
            CurrentViewModel = orgViewModel;
            CurrentViewTitle = "Organizations";
        }

        [RelayCommand]
        private void Navigate(Type viewModelType)
        {
            if (viewModelType == typeof(OrganizationViewModel))
            {
                if (CurrentViewModel is OrganizationViewModel oldViewModel)
                {
                    oldViewModel.OrganizationSelected -= OnOrganizationSelected;
                }

                CurrentViewTitle = "Organizations";
                var orgViewModel = new OrganizationViewModel(_apiService);
                orgViewModel.OrganizationSelected += OnOrganizationSelected;
                CurrentViewModel = orgViewModel;
            }
        }

        private void OnOrganizationSelected(object sender, Organization org)
        {
            if (CurrentViewModel is OrganizationViewModel oldViewModel)
            {
                oldViewModel.OrganizationSelected -= OnOrganizationSelected;
            }

            _navigationStack.Push((CurrentViewModel, CurrentViewTitle));

            // Create dummy program for navigation
            var program = new Program
            {
                Organization = org,
                Name = "Default Program" // You may want to load real program data here
            };

            var programViewModel = new ProgramViewModel(_apiService, org) { Program = program };
            CurrentViewModel = programViewModel;
            CurrentViewTitle = $"Programs - {org.Name}";
            CanNavigateBack = true;
        }

        [RelayCommand]
        private void NavigateBack()
        {
            if (_navigationStack.Count > 0)
            {
                var (viewModel, title) = _navigationStack.Pop();

                if (viewModel is OrganizationViewModel newViewModel)
                {
                    newViewModel.OrganizationSelected += OnOrganizationSelected;
                }

                CurrentViewModel = viewModel;
                CurrentViewTitle = title;
                CanNavigateBack = _navigationStack.Count > 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && CurrentViewModel is OrganizationViewModel viewModel)
                {
                    viewModel.OrganizationSelected -= OnOrganizationSelected;
                }
                _disposed = true;
            }
        }
    }
}