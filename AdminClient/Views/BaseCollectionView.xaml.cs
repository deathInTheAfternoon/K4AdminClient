using System.Windows;
using System.Windows.Controls;

namespace AdminClient.Views
{
    public partial class BaseCollectionView : UserControl
    {
        public BaseCollectionView()
        {
            InitializeComponent();
            // We set the DataContext = this to bind up the props 
            //DataContext = this;
        }

        // 1) CollectionContent - holds our child controls (e.g. DataGrid).
        public static readonly DependencyProperty CollectionContentProperty =
            DependencyProperty.Register(
                nameof(CollectionContent),
                typeof(object),
                typeof(BaseCollectionView),
                new PropertyMetadata(null));

        public object CollectionContent
        {
            get => GetValue(CollectionContentProperty);
            set => SetValue(CollectionContentProperty, value);
        }

        // 2) CollectionTitle
        public static readonly DependencyProperty CollectionTitleProperty =
            DependencyProperty.Register(
                nameof(CollectionTitle),
                typeof(string),
                typeof(BaseCollectionView),
                new PropertyMetadata("My Collection"));

        public string CollectionTitle
        {
            get => (string)GetValue(CollectionTitleProperty);
            set => SetValue(CollectionTitleProperty, value);
        }

        // 3) IsLoading
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(
                nameof(IsLoading),
                typeof(bool),
                typeof(BaseCollectionView),
                new PropertyMetadata(false));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        // 4) ErrorMessage
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register(
                nameof(ErrorMessage),
                typeof(string),
                typeof(BaseCollectionView),
                new PropertyMetadata(null));

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        // 5) CanAdd
        public static readonly DependencyProperty CanAddProperty =
            DependencyProperty.Register(
                nameof(CanAdd),
                typeof(bool),
                typeof(BaseCollectionView),
                new PropertyMetadata(false));

        public bool CanAdd
        {
            get => (bool)GetValue(CanAddProperty);
            set => SetValue(CanAddProperty, value);
        }

        // 6) CanEdit
        public static readonly DependencyProperty CanEditProperty =
            DependencyProperty.Register(
                nameof(CanEdit),
                typeof(bool),
                typeof(BaseCollectionView),
                new PropertyMetadata(false));

        public bool CanEdit
        {
            get => (bool)GetValue(CanEditProperty);
            set => SetValue(CanEditProperty, value);
        }

        // 7) CanDelete
        public static readonly DependencyProperty CanDeleteProperty =
            DependencyProperty.Register(
                nameof(CanDelete),
                typeof(bool),
                typeof(BaseCollectionView),
                new PropertyMetadata(false));

        public bool CanDelete
        {
            get => (bool)GetValue(CanDeleteProperty);
            set => SetValue(CanDeleteProperty, value);
        }

        // 8) AddCommand
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register(
                nameof(AddCommand),
                typeof(System.Windows.Input.ICommand),
                typeof(BaseCollectionView),
                new PropertyMetadata(null));

        public System.Windows.Input.ICommand AddCommand
        {
            get => (System.Windows.Input.ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        // 9) EditCommand
        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(
                nameof(EditCommand),
                typeof(System.Windows.Input.ICommand),
                typeof(BaseCollectionView),
                new PropertyMetadata(null));

        public System.Windows.Input.ICommand EditCommand
        {
            get => (System.Windows.Input.ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        // 10) DeleteCommand
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(
                nameof(DeleteCommand),
                typeof(System.Windows.Input.ICommand),
                typeof(BaseCollectionView),
                new PropertyMetadata(null));

        public System.Windows.Input.ICommand DeleteCommand
        {
            get => (System.Windows.Input.ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
    }
}
