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

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for ProgramCollectionView.xaml
    /// </summary>
    public partial class ProgramCollectionView : UserControl
    {
        public ProgramCollectionView()
        {
            InitializeComponent();

            // Since we want the DataContext to flow through to our BaseCollectionView
            // and its contained DataGrid, we ensure proper binding inheritance
            this.DataContextChanged += (s, e) =>
            {
                // When our DataContext changes, we need to ensure the BaseCollectionView
                // and its children receive the updated context
                if (Content is BaseCollectionView baseView)
                {
                    baseView.DataContext = this.DataContext;
                }
            };
        }
    }
}
