using Normalizing.ViewModels.Pages;
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

namespace Normalizing.Views.Pages
{
    /// <summary>
    /// AutoControlPage.xaml 的交互逻辑
    /// </summary>
    public partial class AutoControlPage : Page
    {
        public AutoControlPage(AutoControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
