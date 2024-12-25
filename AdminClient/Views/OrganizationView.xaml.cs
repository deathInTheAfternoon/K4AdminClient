using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdminClient.Models;
using AdminClient.Services;
using AdminClient.ViewModels;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for OrganizationView.xaml
    /// </summary>
    public partial class OrganizationView : UserControl
    {
        public OrganizationView()
        {
            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            var program = listView?.SelectedItem as Program;
            if (program != null)
            {
                var programViewModel = new ProgramViewModel(App.GetService<ApiService>());
                MainWindowViewModel.NavigateCommand.Execute(new ProgramView { DataContext = programViewModel });
            }
        }
    }
}
