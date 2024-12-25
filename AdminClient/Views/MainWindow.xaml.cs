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
    }
}
