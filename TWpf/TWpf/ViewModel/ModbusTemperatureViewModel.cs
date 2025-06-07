using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
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
using TWpf.Common;
using TWpf.Modbus;
using LiveChartsCore.Defaults;
using Serilog;

namespace TWpf.ViewModel
{
    public partial class TemperatureData : ObservableObject
    {
        public TemperatureData(int salveId)
        {
            SalveId = salveId;
            // 初始化温度图表
            TemperatureSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "温度(℃)",
                    Values = new ObservableCollection<ObservablePoint>(),
                    Fill = null,
                    GeometrySize = 8,
                    LineSmoothness = 0.2
                }
            };

            // 初始化湿度图表
            HumiditySeries = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "湿度(%)",
                    Values = new ObservableCollection<ObservablePoint>(),
                    Fill = null,
                    GeometrySize = 8,
                    LineSmoothness = 0.2
                }
            };

            // 配置X轴(时间轴)
            DateTimeAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name="时间",
                    TextSize=14,
                    NameTextSize=16,
                    Labeler = value => new DateTime((long)value).ToString("HH:mm:ss"), //
                    UnitWidth = TimeSpan.FromMinutes(1).Ticks, //显示单位
                    MinStep = TimeSpan.FromMinutes(1).Ticks,//最小显示单位
                    ForceStepToMin = true,
                    SeparatorsAtCenter = false,
                    MinLimit = DateTime.Now.AddMinutes(-10).Ticks,
                    MaxLimit = DateTime.Now.Ticks//X轴最大显示
                }
            };

            // 配置温度Y轴
            TemperatureAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    MinStep=1,
                    TextSize=14,
                    NameTextSize=16,
                    Name = "温度(℃)",
                    MinLimit = -10,
                    MaxLimit = 50
                }
            };

            // 配置湿度Y轴
            HumidityAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    MinStep = 1,
                    TextSize=14,
                    NameTextSize=16,
                    Name = "湿度(%)",
                    MinLimit = 0,
                    MaxLimit = 100
                }
            };

        }
        public int SalveId { get; set; }
        public string Name { get; set; }

        /// <summary>
        ///  温度
        /// </summary>
        [ObservableProperty]
        private double temperature;

        /// <summary>
        /// 湿度
        /// </summary>
        [ObservableProperty]
        private double humidity;

        /// <summary>
        /// 日期
        /// </summary>
        [ObservableProperty]
        private DateTime time;

        /// <summary>
        /// 历史数据
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<TemperatureData> history;

        public ObservableCollection<ISeries> TemperatureSeries { get; set; }
        public ObservableCollection<ISeries> HumiditySeries { get; set; }
        public ObservableCollection<Axis> DateTimeAxes { get; set; }
        public ObservableCollection<Axis> TemperatureAxes { get; set; }
        public ObservableCollection<Axis> HumidityAxes { get; set; }
        public void SetTemperature(double temperature, double humidity)
        {
            Temperature = temperature;
            Humidity = humidity;
            Time = DateTime.Now;
            var tempValues = (ObservableCollection<ObservablePoint>)TemperatureSeries[0].Values;
            var humidityValues = (ObservableCollection<ObservablePoint>)HumiditySeries[0].Values;

            // 添加新数据点
            tempValues.Add(new ObservablePoint(Time.Ticks, temperature));
            humidityValues.Add(new ObservablePoint(Time.Ticks, humidity));

            // 保持最多30个数据点
            if (tempValues.Count > 60 * 60 * 12)
            {
                tempValues.RemoveAt(0);
                humidityValues.RemoveAt(0);
            }

            // 更新X轴范围
            DateTimeAxes[0].MinLimit = Time.AddMinutes(-10).Ticks;
            DateTimeAxes[0].MaxLimit = Time.Ticks;
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
            TemperatureDatas = new ObservableCollection<TemperatureData> { new TemperatureData(1) };
            Messages = new ObservableCollection<TempMessage>();
            PortNames = SerialPort.GetPortNames().ToList();
            SalveId = 1;
            Rate = 9600;
        }
        [ObservableProperty]
        private string port;
        [ObservableProperty]
        private int salveId;
        [ObservableProperty]
        private int? rate;

        public ObservableCollection<TemperatureData> TemperatureDatas { get; set; }

        public ObservableCollection<TempMessage> Messages { get; set; }

        public List<string> PortNames { get; set; }

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
            if (_rtU.IsOpen)
            {
                AddMessage("串口连接成功");
                return;
            }
            if (_rtU.Connection(Port, Rate.Value, Parity.None, 8, StopBits.One))
            {
                _cts = new CancellationTokenSource();

                CommunicationAsync();

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

        private async Task CommunicationAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000);
                    var res = _rtU.ReadRegisters((byte)SalveId, 0, 2);
                    _dispatcher.Invoke(() =>
                    {
                        var temperature = SensorDataGenerator.ParseTemperature(res.Data);
                        var humidity = SensorDataGenerator.ParseHumidity(res.Data);
                        var data = TemperatureDatas.First(c => c.SalveId == 1);
                        data.SetTemperature((double)temperature, (double)humidity);
                    });
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "读取温湿度异常");
                    _dispatcher.Invoke(() =>
                    {
                        AddMessage("读取异常");
                    });
                }

            }
        }
    }
}
