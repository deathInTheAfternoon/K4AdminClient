using System.Windows;
using System.Windows.Input;
using AdminClient.ViewModels;

namespace AdminClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly RoutedUICommand AdministrateCommand = new RoutedUICommand(
            "Administrate", "Administrate", typeof(MainWindow));
        public static readonly RoutedUICommand TreeViewCommand = new RoutedUICommand(
            "TreeViewCommand", "TreeViewCommand", typeof(MainWindow));
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainWindowViewModel viewModel &&
                e.NewValue is TreeNodeViewModel selectedNode)
            {
                // Convert to async call
                _ = Task.Run(() => viewModel.HandleTreeNodeSelectionAsync(selectedNode));
            }
        }
    }
}
