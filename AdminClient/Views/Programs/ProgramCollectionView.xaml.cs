using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.Grid;

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
        }

        // Don't know how to auto size Syncfusion grid to fit available space. So I'm doing it manually...
        private void DataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is SfDataGrid grid && grid.Columns.Count > 0)
            {
                // Calculate and set the Name column width to fill available space
                var totalWidth = grid.ActualWidth;
                var otherColumnsWidth = 80 + 180; // ID + Actions
                var nameColumnWidth = totalWidth - otherColumnsWidth;

                if (nameColumnWidth > 0)
                {
                    grid.Columns[1].Width = nameColumnWidth;
                }
            }
        }
    }
}
