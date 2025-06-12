using S7.Net;
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

namespace TWpf.Views
{
    /// <summary>
    /// SiemensS7View.xaml 的交互逻辑
    /// </summary>
    public partial class SiemensS7View : Page
    {
        public SiemensS7View()
        {
            InitializeComponent();
        }

        private void Test()
        {
            using var plc = new Plc(CpuType.S71500, "192.168.0.14", 0, 1);
            plc.Open();
            var res = plc.Read("DB1.DBX0.0");
            plc.Write("DB1.DBX0.0", true);
            var res1 = plc.Read("DB1.DBX0.0");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Test();
        }
    }
}
