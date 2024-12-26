using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;

namespace AdminClient.ViewModels
{
    public partial class ProgramCollectionViewModel : BaseCollectionViewModel<Program>
    {
        private readonly Organization _organization;
        public event EventHandler<Program> ProgramSelected;

        public ProgramCollectionViewModel(ApiService apiService, Organization organization)
            : base(apiService)
        {
            _organization = organization;
            CollectionTitle = $"Programs - {organization.Name}";
            LoadDataAsync().ConfigureAwait(false);
        }

        protected override async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var programs = await _apiService.GetProgramsForOrganizationAsync(_organization.Id);
                Items.Clear();
                foreach (var program in programs)
                {
                    Items.Add(program);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading programs: {ex.Message}";
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

                var newProgram = new Program
                {
                    Name = $"New Program {Items.Count + 1}",
                    Organization = _organization
                };

                var createdProgram = await _apiService.CreateProgramAsync(_organization.Id, newProgram);
                Items.Add(createdProgram);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating program: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override async Task EditAsync()
        {
            if (SelectedItem == null) return;

            var dialogViewModel = new EditProgramViewModel(_apiService, SelectedItem);
            var dialog = new EditProgramDialog { DataContext = dialogViewModel };

            dialogViewModel.ProgramUpdated += (s, updatedProgram) =>
            {
                var index = Items.IndexOf(SelectedItem);
                if (index != -1)
                {
                    Items[index] = updatedProgram;
                }
            };

            await DialogHost.Show(dialog);
        }
    }
}