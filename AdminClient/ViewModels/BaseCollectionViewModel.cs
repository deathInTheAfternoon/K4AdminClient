using System.Collections.ObjectModel;
using System.Windows.Input;
using AdminClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdminClient.ViewModels
{
    public abstract partial class BaseCollectionViewModel<T> : ObservableObject, ICollectionViewModel
    {
        protected readonly ApiService _apiService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private ObservableCollection<T> _items = new();

        [ObservableProperty]
        private T _selectedItem;

        [ObservableProperty]
        private string _collectionTitle;

        public virtual bool CanAdd => true;
        public virtual bool CanEdit => SelectedItem != null;
        public virtual bool CanDelete => SelectedItem != null;

        public ICommand AddCommand => new AsyncRelayCommand(AddAsync);
        public ICommand EditCommand => new AsyncRelayCommand(EditAsync, () => CanEdit);
        public ICommand DeleteCommand => new AsyncRelayCommand(DeleteAsync, () => CanDelete);

        protected BaseCollectionViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        protected virtual Task AddAsync() => Task.CompletedTask;
        protected virtual Task EditAsync() => Task.CompletedTask;
        protected virtual Task DeleteAsync() => Task.CompletedTask;

        protected abstract Task LoadDataAsync();
    }
}