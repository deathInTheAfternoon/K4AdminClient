using AdminClient.Models;
using AdminClient.Services;
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
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var newBundle = new BundleDefinition
                {
                    Name = $"New Bundle Definition {Items.Count + 1}",
                    Program = _program,
                    Status = BundleStatus.DRAFT
                };

                var createdBundle = await _apiService.CreateBundleDefinitionAsync(_program.Id, newBundle);
                Items.Add(createdBundle);
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