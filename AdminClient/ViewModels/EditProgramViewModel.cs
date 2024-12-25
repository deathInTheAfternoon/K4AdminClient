using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    public partial class EditProgramViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly Program _originalProgram;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _name;

        public event EventHandler<Program> ProgramUpdated;
        public event EventHandler DialogClosed;

        public EditProgramViewModel(ApiService apiService, Program program)
        {
            _apiService = apiService;
            _originalProgram = program;
            Name = program.Name;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogClosed?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                IsLoading = true;

                var updatedProgram = new Program
                {
                    Id = _originalProgram.Id,
                    Name = Name,
                    Organization = _originalProgram.Organization
                };

                // TODO: Add API endpoint for updating program
                //await _apiService.UpdateProgramAsync(updatedProgram);

                ProgramUpdated?.Invoke(this, updatedProgram);
                DialogClosed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating program: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
