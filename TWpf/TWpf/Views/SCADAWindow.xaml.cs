using CommunityToolkit.Mvvm.Input;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TWpf.Views
{
    /// <summary>
    /// 仪表盘刻度数据类
    /// </summary>
    public class GaugeTick
    {
        public double Angle { get; set; }
        public string Label { get; set; }
        public double Thickness { get; set; }
        public double FontSize { get; set; }
        public Brush Color { get; set; } = Brushes.White;
    }






    /// <summary>
    /// 仪表盘视图模型
    /// </summary>
    public class DashboardViewModel : DependencyObject
    {
        // 速度属性
        public static readonly DependencyProperty SpeedValueProperty =
            DependencyProperty.Register("SpeedValue", typeof(double), typeof(DashboardViewModel),
                new PropertyMetadata(0.0, OnSpeedValueChanged));

        public double SpeedValue
        {
            get { return (double)GetValue(SpeedValueProperty); }
            set { SetValue(SpeedValueProperty, value); }
        }

        // 转速属性
        public static readonly DependencyProperty RpmValueProperty =
            DependencyProperty.Register("RpmValue", typeof(double), typeof(DashboardViewModel),
                new PropertyMetadata(0.0, OnRpmValueChanged));

        public double RpmValue
        {
            get { return (double)GetValue(RpmValueProperty); }
            set { SetValue(RpmValueProperty, value); }
        }

        // 速度表刻度集合
        public List<GaugeTick> SpeedTicks { get; set; }

        // 转速表刻度集合
        public List<GaugeTick> RpmTicks { get; set; }

        // 命令
        public ICommand AccelerateCommand { get; set; }
        public ICommand DecelerateCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        // 定时器
        private DispatcherTimer _timer;
        private double _targetSpeed = 0;
        private double _targetRpm = 0;

        public DashboardViewModel()
        {
            // 初始化速度表刻度
            SpeedTicks = new List<GaugeTick>();
            for (int i = 0; i <= 220; i += 20)
            {
                double angle = i * 240 / 220 - 120;
                SpeedTicks.Add(new GaugeTick
                {
                    Angle = angle,
                    Label = i.ToString(),
                    Thickness = i % 40 == 0 ? 2 : 1,
                    FontSize = i % 40 == 0 ? 14 : 0
                });
            }

            // 初始化转速表刻度
            RpmTicks = new List<GaugeTick>();
            for (int i = 0; i <= 8; i++)
            {
                double angle = i * 240 / 8 - 120;
                Brush color = i < 6 ? Brushes.White : Brushes.Red;

                RpmTicks.Add(new GaugeTick
                {
                    Angle = angle,
                    Label = i.ToString(),
                    Thickness = 2,
                    FontSize = 14,
                    Color = color
                });
            }

            // 初始化命令
            AccelerateCommand = new RelayCommand(Accelerate);
            DecelerateCommand = new RelayCommand(Decelerate);
            ResetCommand = new RelayCommand(Reset);

            // 初始化定时器
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            // 平滑过渡到目标速度和转速
            if (Math.Abs(SpeedValue - _targetSpeed) > 1)
            {
                SpeedValue += (SpeedValue < _targetSpeed) ? 2 : -2;
            }
            else
            {
                SpeedValue = _targetSpeed;
            }

            if (Math.Abs(RpmValue - _targetRpm) > 0.1)
            {
                RpmValue += (RpmValue < _targetRpm) ? 0.1 : -0.1;
            }
            else
            {
                RpmValue = _targetRpm;
            }
        }

        private void Accelerate()
        {
            _targetSpeed = Math.Min(_targetSpeed + 20, 220);
            _targetRpm = Math.Min(_targetRpm + 0.8, 8);
        }

        private void Decelerate()
        {
            _targetSpeed = Math.Max(_targetSpeed - 20, 0);
            _targetRpm = Math.Max(_targetRpm - 0.8, 0);
        }

        private void Reset()
        {
            _targetSpeed = 0;
            _targetRpm = 0;
        }

        private static void OnSpeedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 可以添加速度变化时的逻辑
        }

        private static void OnRpmValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 可以添加转速变化时的逻辑
        }
    }



    /// <summary>
    /// SCADAWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SCADAWindow : Window
    {
        public SCADAWindow()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }


    }
}
