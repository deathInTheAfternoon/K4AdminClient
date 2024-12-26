using System.Windows.Input;

namespace AdminClient.ViewModels
{
    public interface ICollectionViewModel
    {
        bool IsLoading { get; }
        string ErrorMessage { get; }
        string CollectionTitle { get; }
        bool CanAdd { get; }
        bool CanEdit { get; }
        bool CanDelete { get; }
        ICommand AddCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}