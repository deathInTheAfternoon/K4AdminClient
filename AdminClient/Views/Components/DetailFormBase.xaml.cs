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

namespace AdminClient.Views.Components
{
    /// <summary>
    /// Interaction logic for DetailFormBase.xaml
    /// </summary>
    public partial class DetailFormBase : UserControl
    {
        public DetailFormBase()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DetailContentProperty = DependencyProperty.Register("FormContent", typeof(object), typeof(DetailFormBase));

        public object DetailContent
        {
            get => GetValue(DetailContentProperty);
            set => SetValue(DetailContentProperty, value);
        }
    }
}
