using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    public partial class CreateBundleDefinitionDialogModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly Program _program;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _name;

        // Event emitters
        public event EventHandler<BundleDefinition> BundlesCollectionUpdated;
        public event EventHandler DialogClosed;

        public CreateBundleDefinitionDialogModel(ApiService apiService, Program program)
        {
            _apiService = apiService;
            _program = program;
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
                System.Windows.MessageBox.Show("Bundle name cannot be empty");
                return;
            }

            try
            {
                IsLoading = true;

                var newBundle = new BundleDefinition
                {
                    Name = Name,
                    Program = _program,
                    Status = BundleStatus.DRAFT
                };
                var createdBundle = await _apiService.CreateBundleDefinitionAsync(_program.Id, newBundle);
                
                // Invoke client handlers
                BundlesCollectionUpdated?.Invoke(this, createdBundle);
                DialogClosed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error creating bundle definition: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}