using System.Windows.Controls;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for OperatingUnitCollectionView.xaml
    /// </summary>
    public partial class OperatingUnitCollectionView : UserControl
    {
        public OperatingUnitCollectionView()
        {
            InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                if (Content is BaseCollectionView baseView)
                {
                    baseView.DataContext = this.DataContext;
                }
            };
        }
    }
}
