using System.Windows.Controls;

namespace AdminClient.Views
{
    public partial class OrganizationCollectionView : UserControl
    {
        public OrganizationCollectionView()
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