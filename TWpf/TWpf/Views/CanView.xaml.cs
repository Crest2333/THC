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
using TWpf.ViewModel;

namespace TWpf.Views
{
    /// <summary>
    /// CanView.xaml 的交互逻辑
    /// </summary>
    public partial class CanView : Page
    {
        public CanView()
        {
            InitializeComponent();
            DataContext = new CanViewModel();
            Unloaded += CanView_Unloaded;
        }

        private void CanView_Unloaded(object sender, RoutedEventArgs e)
        {
            var vm = (CanViewModel)DataContext;
            vm.Dispose();
        }
    }
}
