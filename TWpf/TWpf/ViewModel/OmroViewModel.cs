using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TWpf.Model;

namespace TWpf.ViewModel
{
    /// <summary>
    /// C-Mode
    /// 发送指令
    /// |起始符号|节点号|操作内容|校验码|结束符|
    /// 接收指令
    /// |起始符号|节点号|状态码|操作内容|校验码|结束符|
    /// </summary>
    public partial class OmroViewModel : ObservableObject
    {
        private readonly Dispatcher _dispatcher;
        [ObservableProperty]
        private PortInfo portInfo;
        private readonly SerialPort serialPort;
        [ObservableProperty]
        private string unit;
        [ObservableProperty]
        private string startAddress;
        [ObservableProperty]
        private string data;
        [ObservableProperty]
        private string command;

        public ObservableCollection<string> Datas { get; } = new ObservableCollection<string>();
        public OmroViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            portInfo = new PortInfo("COM4", 9600, System.IO.Ports.Parity.None, 7, System.IO.Ports.StopBits.Two);
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived;
            Command = "RD";
            Unit = "00";
            Data = "00000005";
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("123");
        }

        [RelayCommand]
        private void Connect()
        {
            serialPort.PortName = portInfo.PortName;
            serialPort.Parity = portInfo.Parity;
            serialPort.BaudRate = portInfo.BaudRate;
            serialPort.DataBits = portInfo.DataBits;
            serialPort.StopBits = portInfo.StopBits;
            serialPort.Open();
        }

        [RelayCommand]
        private void Send()
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("串口未打开");
                return;
            }
            string cmd = BuildCommand(unit, command, Data);
            // @00RD0000000523*CR
            string response = SendData(cmd);

            _dispatcher.Invoke(() =>
            {
                Datas.Add($"发送：{cmd} 响应：{response}");
            });
        }
        private string SendData(string cmd)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(cmd);
            serialPort.Write(buffer, 0, buffer.Length);
            // 读取响应
            StringBuilder response = new StringBuilder();
            DateTime timeout = DateTime.Now.AddSeconds(2);

            while (DateTime.Now < timeout)
            {
                if (serialPort.BytesToRead > 0)
                {
                    response.Append(serialPort.ReadExisting());
                    if (response.ToString().Contains("*\r")) break;
                }
                System.Threading.Thread.Sleep(50);
            }
            return response.ToString();
        }

        private string BuildCommand(string unit, string command, string data)
        {
            // 示例命令格式: @00RD00020002*
            string content = $"{unit}{command}{data}";
            string fcs = CalculateFCS(content);
            return $"@{content}{fcs}*\r";
        }

        private string CalculateFCS(string data)
        {
            byte[] buffers = Encoding.ASCII.GetBytes(data);
            byte temp = buffers[0];
            for (int i = 0; i < buffers.Length; i++)
            {
                temp ^= buffers[i];
            }
            return Convert.ToByte(temp.ToString(), 16).ToString().ToUpper();
        }
    }
}
