using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TWpf.Model;
using TWpf.Views;

namespace TWpf.ViewModel
{
    public partial class BarcodeScannerViewModel : ObservableObject
    {
        [ObservableProperty]
        private BarcodeScannerInfo barcodeScannerInfo;
        [ObservableProperty]
        private bool disableConnectBtn;
        [ObservableProperty]
        private bool disableDisConnectBtn;
        public ObservableCollection<BarcodeScannerData> Barcodes { get; } = new ObservableCollection<BarcodeScannerData>();

        private SerialPort serialPort;
        private readonly Dispatcher _dispatcher;
        private readonly StringBuilder receivedDataBuffer = new StringBuilder();
        private const string DataFrameSeparator = "\r\n";

        private object bufferLock = new object();

        public BarcodeScannerViewModel(Dispatcher dispatcher)
        {
            this.barcodeScannerInfo = new BarcodeScannerInfo("COM19", 9600, System.IO.Ports.Parity.None, 8, StopBits.One);
            serialPort = new SerialPort();
            serialPort.DataReceived += PortDataReceived;
            _dispatcher = dispatcher;
            disableDisConnectBtn = false;
            disableConnectBtn = true;
        }

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs eventArgs)
        {
            SerialPort sp = (SerialPort)sender;
            string receivedData = sp.ReadExisting();
            receivedDataBuffer.Append(receivedData);
            // 直接读取receivedData，数据会不完整，根据\r\n判断数据是否完整
            // 处理数据帧
            while (receivedDataBuffer.ToString().Contains(DataFrameSeparator))
            {
                int separatorIndex = receivedDataBuffer.ToString().IndexOf(DataFrameSeparator);
                string dataFrame = receivedDataBuffer.ToString().Substring(0, separatorIndex);
                receivedDataBuffer.Remove(0, separatorIndex + 2);

                // 在 UI 线程中更新数据
                _dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(dataFrame))
                    {
                        lock (bufferLock)
                        {
                            Barcodes.Add(new BarcodeScannerData(dataFrame, DateTime.Now));
                        }
                    }
                });
            }
        }

        [RelayCommand]
        private void OpenSetting()
        {
            BarcodeScannerSettingsWindow settingsWindow = new BarcodeScannerSettingsWindow(this);
            settingsWindow.ShowDialog();

        }

        [RelayCommand]
        private void Disconnect()
        {
            if (!serialPort.IsOpen)
            {
                return;
            }
            serialPort.Close();
            DisableDisConnectBtn = false;
            DisableConnectBtn = true;

        }
        [RelayCommand]
        private void ClearList()
        {
            lock (bufferLock)
            {
                Barcodes.Clear();
            }
        }
        [RelayCommand]
        private void AddBarcode(string barcode)
        {
            lock (bufferLock)
            {
                Barcodes.Add(new BarcodeScannerData(barcode, DateTime.Now));
            }
        }

        [RelayCommand]
        private void Connect()
        {
            if (serialPort.IsOpen)
            {
                return;
            }
            serialPort.PortName = barcodeScannerInfo.PortName;
            serialPort.Parity = barcodeScannerInfo.Parity;
            serialPort.BaudRate = barcodeScannerInfo.BaudRate;
            serialPort.DataBits = barcodeScannerInfo.DataBits;
            serialPort.StopBits = barcodeScannerInfo.StopBits;
            serialPort.Open();
            DisableConnectBtn = false;
            DisableDisConnectBtn = true;

        }
    }
}
