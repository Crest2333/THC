using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RTU;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TWpf.Modbus;

namespace TWpf.ViewModel
{
    public partial class ModbusRTUViewModel : ObservableObject
    {
        private readonly ModbusRTU _modbusRTU;
        private readonly Dispatcher _dispatcher;

        public ModbusRTUViewModel(ModbusRTU modbusRTU, Dispatcher dispatcher)
        {
            _modbusRTU = modbusRTU;
            _dispatcher = dispatcher;
            FunctionTypeList = new ObservableCollection<FunctionType>(Enum.GetValues<FunctionType>());
            Content = new ObservableCollection<RtuMessage>();
        }


        [ObservableProperty]
        private FunctionType functionType;
        [ObservableProperty]
        private byte slaveId;
        [ObservableProperty]
        private ushort startAddress;
        [ObservableProperty]
        private ushort length;
        [ObservableProperty]
        private ushort writeValue;
        public ObservableCollection<FunctionType> FunctionTypeList { get; set; }

        public ObservableCollection<RtuMessage> Content { get; set; }


        [RelayCommand]
        public void Read()
        {
            byte[] data = null;
            switch (FunctionType)
            {
                case FunctionType.ReadInputCoil:
                    data = _modbusRTU.ReadInputCoil(SlaveId, StartAddress, Length);
                    break;
                case FunctionType.ReadOutputCoil:
                    data = _modbusRTU.ReadOutputCoil(SlaveId, StartAddress, Length);
                    break;
                case FunctionType.ReadOutputRegister:
                    data = _modbusRTU.ReadHoldingRegisters(SlaveId, StartAddress, Length);
                    break;
                case FunctionType.ReadInputRegister:
                    data = _modbusRTU.ReadInputRegisters(SlaveId, StartAddress, Length);
                    break;
                default:
                    break;
            }
            if (data == null)
            {
                return;
            }
            var output = new StringBuilder();
            foreach (var item in data)
            {
                output.Append($"{item:X} ");
            }

            _dispatcher.Invoke(() =>
            {
                Content.Add(new RtuMessage
                {
                    Time = DateTime.Now,
                    Type = "接收",
                    Content = output.ToString()
                });
            });
        }

        [RelayCommand]
        public void Write()
        {
            byte[] data = null;

            switch (FunctionType)
            {
                case FunctionType.WriteSingleCoil:
                    data = _modbusRTU.WriteSingleCoil(SlaveId, StartAddress, true);
                    break;
                case FunctionType.WriteSingleRegister:
                    data = _modbusRTU.WriteSingleRegister(SlaveId, StartAddress, WriteValue);
                    break;
                case FunctionType.WriteMultipleCoil:
                    break;
                case FunctionType.WriteMultipleRegister:
                    break;
            }

            if (data == null)
            {
                return;
            }
            var output = new StringBuilder();
            foreach (var item in data)
            {
                output.Append($"{item:X} ");
            }

            _dispatcher.Invoke(() =>
            {
                Content.Add(new RtuMessage
                {
                    Time = DateTime.Now,
                    Type = "接收",
                    Content = output.ToString()
                });
            });
        }
    }

    public class RtuMessage
    {
        public DateTime Time { get; set; }

        public string Content { get; set; }

        public string Type { get; set; }
    }
}
