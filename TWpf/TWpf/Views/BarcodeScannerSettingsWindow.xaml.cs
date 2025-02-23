using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Shapes;
using TWpf.ViewModel;

namespace TWpf.Views
{
    /// <summary>
    /// BarcodeScannerSettings.xaml 的交互逻辑
    /// </summary>
    public partial class BarcodeScannerSettingsWindow : Window
    {
        private readonly BarcodeScannerViewModel _barcodeScannerViewModel;

        public BarcodeScannerSettingsWindow(BarcodeScannerViewModel barcodeScannerViewModel)
        {
            InitializeComponent();
            _barcodeScannerViewModel = barcodeScannerViewModel;
            // 设置端口号初始值
            SetComboBoxInitialValue(PortNameComboBox, _barcodeScannerViewModel.BarcodeScannerInfo.PortName);
            // 设置波特率初始值
            SetComboBoxInitialValue(BaudRateComboBox, _barcodeScannerViewModel.BarcodeScannerInfo.BaudRate.ToString());
            // 设置校验位初始值
            SetComboBoxInitialValue(ParityComboBox, ConvertParityDesc(_barcodeScannerViewModel.BarcodeScannerInfo.Parity));
            // 设置数据位初始值
            SetComboBoxInitialValue(DataBitsComboBox, _barcodeScannerViewModel.BarcodeScannerInfo.DataBits.ToString());
            // 设置停止位初始值
            SetComboBoxInitialValue(StopBitsComboBox, ((int)(_barcodeScannerViewModel.BarcodeScannerInfo.StopBits)).ToString());
        }

        private void SetComboBoxInitialValue(ComboBox comboBox, string value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == value)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }


        private static Parity ConvertParity(string parityDesc)
        {
            switch (parityDesc)
            {
                case "无":
                    return Parity.None;
                case "奇校验":
                    return Parity.Odd;
                case "偶校验":
                    return Parity.Even;
                default:
                    return Parity.None;
            }
        }

        private static string ConvertParityDesc(Parity parity)
        {
            switch (parity)
            {
                case Parity.None:
                    return "无";
                case Parity.Odd:
                    return "奇校验";
                case Parity.Even:
                    return "偶校验";
                default:
                    return "无";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var portName = (PortNameComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var baudRate = Convert.ToInt32((BaudRateComboBox.SelectedItem as ComboBoxItem)?.Content.ToString());
            var parity = (ParityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var dataBits = Convert.ToInt32((DataBitsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString());
            var stopBits = (StopBits)Convert.ToInt32((StopBitsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString());

            _barcodeScannerViewModel.BarcodeScannerInfo.PortName = portName;
            _barcodeScannerViewModel.BarcodeScannerInfo.BaudRate = baudRate;
            _barcodeScannerViewModel.BarcodeScannerInfo.Parity = ConvertParity(parity);
            _barcodeScannerViewModel.BarcodeScannerInfo.DataBits = dataBits;
            _barcodeScannerViewModel.BarcodeScannerInfo.StopBits = stopBits;
            this.Close();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
