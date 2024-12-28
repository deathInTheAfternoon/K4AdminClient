using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    public partial class CreateProgramDialogModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly Organization _organization;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _name;

        // Event emitters
        public event EventHandler<Program> ProgramsCollectionUpdated;
        public event EventHandler DialogClosed;

        public CreateProgramDialogModel(ApiService apiService, Organization organization)
        {
            _apiService = apiService;
            _organization = organization;
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
                System.Windows.MessageBox.Show("Program name cannot be empty");
                return;
            }

            try
            {
                IsLoading = true;

                var newOrg = new Program { Name = Name };
                var createdOrg = await _apiService.CreateProgramAsync(_organization.Id, newOrg);

                // Invoke client handlers
                ProgramsCollectionUpdated?.Invoke(this, createdOrg);
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