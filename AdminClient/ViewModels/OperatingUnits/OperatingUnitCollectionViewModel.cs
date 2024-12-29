using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace AdminClient.ViewModels
{
    public partial class OperatingUnitCollectionViewModel : BaseCollectionViewModel<OperatingUnit>
    {
        private readonly Program _program;
        public event EventHandler<OperatingUnit> OperatingUnitSelected;

        [ObservableProperty]
        private ObservableCollection<User> _hcps = new();

        [ObservableProperty]
        private ObservableCollection<User> _subjects = new();

        public OperatingUnitCollectionViewModel(ApiService apiService, Program program)
            : base(apiService)
        {
            _program = program;
            CollectionTitle = $"Operating Units - {program.Name}";
            LoadDataAsync().ConfigureAwait(false);
        }

        protected override async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var operatingUnits = await _apiService.GetOperatingUnitsForProgramAsync(_program.Id);
                Items.Clear();
                foreach (var unit in operatingUnits)
                {
                    Items.Add(unit);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading operating units: {ex.Message}";
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

                var newUnit = new OperatingUnit
                {
                    Name = $"New Operating Unit {Items.Count + 1}",
                    Program = _program
                };

                var createdUnit = await _apiService.CreateOperatingUnitAsync(_program.Id, newUnit);
                Items.Add(createdUnit);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating operating unit: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ViewHcps(OperatingUnit operatingUnit)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                // TODO: Add API endpoint for getting HCPs
                // var hcps = await _apiService.GetHcpsForOperatingUnitAsync(operatingUnit.Id);

                // For now, showing a dialog indicating the feature is coming soon
                await DialogHost.Show(new MaterialDesignThemes.Wpf.Card
                {
                    Content = new TextBlock
                    {
                        Text = "HCP viewing coming soon",
                        Margin = new System.Windows.Thickness(16)
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading HCPs: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ViewSubjects(OperatingUnit operatingUnit)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                // TODO: Add API endpoint for getting Subjects
                // var subjects = await _apiService.GetSubjectsForOperatingUnitAsync(operatingUnit.Id);

                // For now, showing a dialog indicating the feature is coming soon
                await DialogHost.Show(new MaterialDesignThemes.Wpf.Card
                {
                    Content = new TextBlock
                    {
                        Text = "Subject viewing coming soon",
                        Margin = new System.Windows.Thickness(16)
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading subjects: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}