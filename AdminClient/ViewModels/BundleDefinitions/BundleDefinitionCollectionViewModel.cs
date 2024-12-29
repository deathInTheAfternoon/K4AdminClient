using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
using MaterialDesignThemes.Wpf;

namespace AdminClient.ViewModels
{
    public partial class BundleDefinitionCollectionViewModel : BaseCollectionViewModel<BundleDefinition>
    {
        private readonly Program _program;
        // Event emitters
        public event EventHandler<BundleDefinition> BundlesCollectionUpdated;

        public BundleDefinitionCollectionViewModel(ApiService apiService, Program program)
            : base(apiService)
        {
            _program = program;
            CollectionTitle = $"Bundle Definitions - {program.Name}";
            LoadDataAsync().ConfigureAwait(false);
        }

        protected override async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var bundles = await _apiService.GetBundleDefinitionsForProgramAsync(_program.Id);
                Items.Clear();
                foreach (var bundle in bundles)
                {
                    Items.Add(bundle);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading bundle definitions: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // On UI button click handler.
        protected override async Task AddAsync()
        {
            // Create and setup dialog
            var dialogViewModel = new CreateBundleDefinitionDialogModel(_apiService, _program);
            var dialog = new CreateBundleDefinitionDialog { DataContext = dialogViewModel };

            try
            {
                IsLoading = true;
                ErrorMessage = null;

                dialogViewModel.BundlesCollectionUpdated += (s, newBundle) =>
                {
                    Items.Add(newBundle);
                    // Raise event to notify listeners
                    BundlesCollectionUpdated?.Invoke(this, newBundle);
                };

                dialogViewModel.DialogClosed += (s, e) => DialogHost.CloseDialogCommand.Execute(null, null); ;

                // Show dialog and wait for result
                await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "RootDialog");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating bundle definition: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}