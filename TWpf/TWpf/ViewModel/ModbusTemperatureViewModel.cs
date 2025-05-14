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
        public TemperatureData()
        {
            History = new List<TemperatureData>();
        }
        /// <summary>
        ///  温度
        /// </summary>
        public decimal Temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public decimal Humidity { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 历史数据
        /// </summary>
        public List<TemperatureData> History { get; set; }

        public void SetTemperature(decimal temperature, decimal humidity)
        {
            Temperature = temperature;
            Humidity = humidity;
            Time = DateTime.Now;
        }

    }

    public class TempMessage
    {
        public string Message { get; set; }

        public DateTime Time { get; set; }

    }
    public partial class ModbusTemperatureViewModel : ObservableObject
    {
        private readonly ModbusRTUAdvance _rtU;
        public ModbusTemperatureViewModel(Dispatcher dispatcher)
        {
            _rtU = new ModbusRTUAdvance();
            _dispatcher = dispatcher;
            TemperatureData1 = new TemperatureData();
            TemperatureData2 = new TemperatureData();
            Messages = new ObservableCollection<TempMessage>();
        }
        [ObservableProperty]
        private string port;
        [ObservableProperty]
        private int? rate;

        [ObservableProperty]
        private TemperatureData temperatureData1;
        [ObservableProperty]
        private TemperatureData temperatureData2;

        public ObservableCollection<TempMessage> Messages { get; set; }

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
                Task.Run(() =>
                {
                    Communication();
                });
                AddMessage("串口连接成功");
            }
            else
            {
                AddMessage("串口连接失败");
            }
        }

        private void AddMessage(string message)
        {
            Messages.Add(new TempMessage { Message = message, Time = DateTime.Now });
        }

        [RelayCommand]
        public void DisConnect()
        {
            _cts.Cancel();
            _rtU.Dispose();
            AddMessage("串口断开");
        }


        private void Communication()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    Task.Delay(1000);
                    var res = _rtU.ReadRegisters(1, 0, 2);
                    _dispatcher.Invoke(() =>
                    {
                        var temperature = (res.Data[0] * 256 + res.Data[1]) * 0.1M;
                        var humidity = (res.Data[2] * 256 + res.Data[3]) * 0.1M;
                        var data = new TemperatureData();

                        data.SetTemperature(temperature, humidity);
                        TemperatureData1 = data;
                    });

                    var res2 = _rtU.ReadRegisters(2, 0, 2);
                    _dispatcher.Invoke(() =>
                    {
                        var temperature = (res2.Data[0] * 256 + res2.Data[1]) * 0.1M;
                        var humidity = (res2.Data[2] * 256 + res2.Data[3]) * 0.1M;
                        var data = new TemperatureData();

                        data.SetTemperature(temperature, humidity);
                        TemperatureData2 = data;
                    });
                }
                catch (Exception ex)
                {
                    _dispatcher.Invoke(() =>
                    {
                        AddMessage("读取异常");
                    });
                }

            }
        }
    }
}
