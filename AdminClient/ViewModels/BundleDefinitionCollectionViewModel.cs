using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AdminClient.ViewModels
{
    public partial class BundleDefinitionCollectionViewModel : BaseCollectionViewModel<BundleDefinition>
    {
        private readonly Program _program;

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

        protected override async Task AddAsync()
        {
            // Create and setup dialog
            var dialogViewModel = new CreateBundleDefinitionViewModel(_apiService, _program);
            var dialog = new CreateBundleDefinitionDialog { DataContext = dialogViewModel };

            try
            {
                IsLoading = true;
                ErrorMessage = null;

                dialogViewModel.BundleCreated += (s, newBundle) =>
                {
                    Items.Add(newBundle);
                };

                // Show dialog and wait for result
                await MaterialDesignThemes.Wpf.DialogHost.Show(dialog);
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