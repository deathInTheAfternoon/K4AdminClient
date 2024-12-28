using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdminClient.Models;
using AdminClient.Services;
using AdminClient.Views;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;

namespace AdminClient.ViewModels
{
    public partial class ProgramCollectionViewModel : BaseCollectionViewModel<Program>
    {
        private readonly Organization _organization;
        // Event emitters
        public event EventHandler<Program> ProgramsCollectionUpdated;
        public event EventHandler<Program> ProgramDeleted;

        // Todo: orginally these were used to enabled the 'Action Bar' delete button - which now just hosts 'Add'.
        // Since these buttons are now always enabled in the datagrid - need to analyze usage.
        public override bool CanDelete => true;
        public override bool CanEdit => true;

        public ProgramCollectionViewModel(ApiService apiService, Organization organization)
            : base(apiService)
        {
            _organization = organization;
            CollectionTitle = $"Programs ({organization.Name})";
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

                // Create and setup dialog
                var dialogViewModel = new CreateProgramDialogModel(_apiService, _organization);
                var dialog = new CreateProgramDialog { DataContext = dialogViewModel };

                // Register our event handler lambdas
                dialogViewModel.ProgramsCollectionUpdated += (s, newOrg) =>
                {
                    Items.Add(newOrg);
                    // Raise event to notify listeners
                    ProgramsCollectionUpdated?.Invoke(this, newOrg);
                };

                dialogViewModel.DialogClosed += (s, e) => DialogHost.CloseDialogCommand.Execute(null, null); ;

                // Show dialog and wait for result
                await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "RootDialog");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating organization: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Edit-command handler
        protected override async Task EditAsync()
        {
            if (SelectedItem == null) return;
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                // Setup dialog with lambda event handler
                var dialogViewModel = new EditProgramViewModel(_apiService, SelectedItem);
                var dialog = new EditProgramDialog { DataContext = dialogViewModel };

                // Register our event handler lambdas
                dialogViewModel.ProgramUpdated += (s, updatedOrg) =>
                {
                    // Update the item in the collection
                    var index = Items.IndexOf(SelectedItem);
                    if (index != -1)
                    {
                        Items[index] = updatedOrg;
                        ProgramsCollectionUpdated?.Invoke(this, null);
                    }
                };

                dialogViewModel.DialogClosed += (s, e) => DialogHost.CloseDialogCommand.Execute(null, null); ;

                // Show dialog and wait for result
                await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "RootDialog");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating organization: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Override the existing protected DeleteAsync method
        protected override async Task DeleteAsync()
        {
            if (SelectedItem == null) return;

            // IMPORTANT: Store a reference to item which will not be nulled and can be safely referenced by the ProgramDeleted event.
            // SelecteItem will be nulled after the dialog is closed.
            var itemToDelete = SelectedItem;

            #region Dialog Box Implementation
            // Create a proper dialog with buttons
            var dialogContent = new StackPanel { Margin = new Thickness(16) };
            dialogContent.Children.Add(new TextBlock
            {
                Text = $"Are you sure you want to delete organization '{SelectedItem.Name}'?",
                Margin = new Thickness(0, 0, 0, 16)
            });

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var cancelButton = new Button
            {
                Content = "CANCEL",
                Style = Application.Current.FindResource("MaterialDesignFlatButton") as Style,
                Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand,
                CommandParameter = false,
                Margin = new Thickness(0, 0, 8, 0)
            };

            var deleteButton = new Button
            {
                Content = "DELETE",
                Style = Application.Current.FindResource("MaterialDesignFlatButton") as Style,
                Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand,
                CommandParameter = true,
                Foreground = Brushes.Red
            };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(deleteButton);
            dialogContent.Children.Add(buttonPanel);

            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(dialogContent, "RootDialog");

            if (result is not bool confirmed || !confirmed) return;

            #endregion

            try
            {
                IsLoading = true;
                ErrorMessage = null;

                await _apiService.DeleteProgramAsync(itemToDelete.Organization.Id, itemToDelete);

                // Remove from collection
                Items.Remove(itemToDelete);
                // Raise event to notify listeners
                ProgramDeleted?.Invoke(this, itemToDelete);
                // As mentioned, this is why we mustn't pass SelectedItem directly to the event...else handler would receive a null value.
                SelectedItem = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting organization: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task ViewDetails(Program org)
        {
            var dialog = new ProgramDetailsDialog { DataContext = org };
            await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "RootDialog");
        }
    }
}