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
using System.Windows;
using System.Windows.Threading;
using TWpf.Modbus;

namespace TWpf.ViewModel
{
    public class TemperatureData
    {
        /// <summary>
        ///  温度
        /// </summary>
        public decimal Temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public decimal Humidity { get; set; }

    }
    public partial class ModbusTemperatureViewModel : ObservableObject
    {
        private readonly ModbusRTUAdvance _rtU;
        public ModbusTemperatureViewModel(Dispatcher dispatcher)
        {
            _rtU = new ModbusRTUAdvance();
            _dispatcher = dispatcher;
            TemperatureData1 = new TemperatureData
            {
                Humidity = 12.2m
            };
        }
        [ObservableProperty]
        private string port;
        [ObservableProperty]
        private int? rate;

        [ObservableProperty]
        private TemperatureData temperatureData1;

        private readonly Dispatcher _dispatcher;

        private CancellationTokenSource _cts;

        [RelayCommand]
        public void Connect()
        {
            if (string.IsNullOrWhiteSpace(Port))
            {
                MessageBox.Show("串口号必填");
                return;
            }
            if (!Rate.HasValue)
            {
                MessageBox.Show("波特率必选");
                return;
            }

            if (_rtU.Connection(Port, Rate.Value, Parity.None, 8, StopBits.One))
            {
                _cts = new CancellationTokenSource();
                Task.Run(Communication);
                MessageBox.Show("连接成功");
            }
            else
            {
                MessageBox.Show("连接失败");
            }
        }

        [RelayCommand]
        public void DisConnect()
        {
            _rtU.Dispose();
        }

        private void Communication()
        {
            while (!_cts.IsCancellationRequested)
            {
                Task.Delay(1000);
                try
                {
                    var res = _rtU.ReadRegisters(1, 0, 2);
                    _dispatcher.Invoke(() =>
                    {
                        TemperatureData1 = new TemperatureData
                        {
                            Temperature = (res.Data[0] * 256 + res.Data[1]) * 0.1M,
                            Humidity = (res.Data[2] * 256 + res.Data[3]) * 0.1M
                        };
                    });
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
