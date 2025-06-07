using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using TWpf.Modbus;

namespace TWpf.ViewModel
{
    public partial class ModbusTcpViewModel : ObservableObject
    {
        private readonly Dispatcher _dispatcher;
        private readonly ModbusTCP _client;
        [ObservableProperty]
        private string ipAddress = "127.0.0.1";
        [ObservableProperty]
        private int port = 502;
        [ObservableProperty]
        private int timeout = 1000;
        [ObservableProperty]
        private byte slaveId = 1;
        [ObservableProperty]
        private string connectionStatusText = "未连接";
        [ObservableProperty]
        private Brush connectionStatus = Brushes.Gray;

        [ObservableProperty]
        private FunctionType selectedFunctionCode;
        [ObservableProperty]
        private ushort quantity;
        [ObservableProperty]
        private ushort startAddress;
        [ObservableProperty]
        private ushort writeValue;
        [ObservableProperty]
        private string writeValues;

        public ObservableCollection<ModbusTcpMessage> ReceivedData { get; set; }

        public ObservableCollection<DeviceData> DataItems { get; set; }

        public ObservableCollection<LogItem> LogItems { get; set; }

        public ModbusTcpViewModel(Dispatcher dispatcher)
        {
            _client = new ModbusTCP();
            _dispatcher = dispatcher;
            ReceivedData = new ObservableCollection<ModbusTcpMessage>();
            LogItems = new ObservableCollection<LogItem>();
            DataItems = new ObservableCollection<DeviceData>();
            SelectedFunctionCode = FunctionType.ReadOutputRegister;
        }

        private void AddLog(string message)
        {
            LogItems.Add(new LogItem { Message = message, Time = DateTime.Now });
        }

        [RelayCommand]
        public async Task ConnectAsync()
        {
            try
            {
                await _client.ConnectAsync(IpAddress, Port, Timeout);
                ConnectionStatusText = "连接成功！";
                AddLog("连接成功");
            }
            catch (Exception ex)
            {
                AddLog("连接失败");
            }
        }

        [RelayCommand]
        public async Task ReadAsync()
        {
            try
            {
                var result = await _client.ReadRegisterAsync(SlaveId, SelectedFunctionCode, StartAddress, Quantity);
                UpdateDataItem(result);
                ReceivedData.Add(result);
            }
            catch (Exception ex)
            {
                AddLog("发送失败");
            }
        }

        [RelayCommand]
        public async Task WriteAsync()
        {
            try
            {
                var result = await _client.WriteRegisterAsync(SlaveId, SelectedFunctionCode, StartAddress, WriteValue);
                ReceivedData.Add(result);
            }
            catch (Exception ex)
            {
                AddLog("写入失败");
            }
        }

        private void UpdateDataItem(ModbusTcpMessage modbusTcpMessage)
        {

            for (int i = 0; i < modbusTcpMessage.Data.Length; i += 2)
            {
                DataItems.Add(new DeviceData
                {
                    Address = modbusTcpMessage.StartAddress + i,
                    Value = (ushort)((modbusTcpMessage.Data[i] << 8) | modbusTcpMessage.Data[i + 1]),
                });
            }
        }
    }

    public class LogItem
    {
        public DateTime Time { get; set; }

        public string Message { get; set; }
    }
}
