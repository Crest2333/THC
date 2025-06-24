using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.ViewModels.Pages
{
    public partial class AutoControlViewModel : ObservableObject
    {
        public AutoControlViewModel()
        {
                
        }

        // 位移传感器参数
        [ObservableProperty]
        private string _ySensor;
        /// <summary>
        /// Y轴焊缝零位
        /// </summary>
        [ObservableProperty]
        private string _yWeldZero;

        /// <summary>
        /// Y轴补偿零位
        /// </summary>
        [ObservableProperty]
        private string _yCompensationZero;

        /// <summary>
        /// 补偿使能
        /// </summary>
        [ObservableProperty]
        private bool _compensationEnabled;

        /// <summary>
        /// Z轴传感器
        /// </summary>
        [ObservableProperty]
        private string _zSensor;

        /// <summary>
        /// 焊缝高度
        /// </summary>
        [ObservableProperty]
        private string _weldHeight;

        /// <summary>
        /// Z轴补偿零位
        /// </summary>
        [ObservableProperty]
        private string _zCompensationZero;

        /// <summary>
        /// 找焊缝使能
        /// </summary>
        [ObservableProperty]
        private bool _weldDetectionEnabled;

        // 运动控制参数
        /// <summary>
        /// 起始位置
        /// </summary>
        [ObservableProperty]
        private string _startPosition;

        [ObservableProperty]
        private string _startPositionSpeed;

        [ObservableProperty]
        private string _weldLimit;

        [ObservableProperty]
        private string _weldDetectionSpeed;

        [ObservableProperty]
        private string _heatingDistance;

        [ObservableProperty]
        private string _heatingPositionSpeed;

        [ObservableProperty]
        private string _airSprayDistance;

        [ObservableProperty]
        private string _airSpraySpeed;

        // 工艺参数 - 起始状态
        [ObservableProperty]
        private string _initialTemperature;

        [ObservableProperty]
        private string _initialPower;

        [ObservableProperty]
        private string _initialCoolingParam;

        // 工艺参数 - 转变状态
        [ObservableProperty]
        private string _transitionTemperature;

        [ObservableProperty]
        private string _transitionPower;

        [ObservableProperty]
        private string _transitionCoolingParam;

        // 工艺参数 - 最终状态
        [ObservableProperty]
        private string _finalTemperature;

        [ObservableProperty]
        private string _finalPower;

        // 冷却参数上下限
        [ObservableProperty]
        private string _pressureUpperLimit;

        [ObservableProperty]
        private string _flowUpperLimit;

        [ObservableProperty]
        private string _energyUpperLimit;

        [ObservableProperty]
        private string _pressureLowerLimit;

        [ObservableProperty]
        private string _flowLowerLimit;

        [ObservableProperty]
        private string _energyLowerLimit;

        // 工艺参数管理
        [ObservableProperty]
        private string _processParameterName;

        [ObservableProperty]
        private string _selectedProcessParameter;

        public ObservableCollection<string> ProcessParameters { get; } = new();

        // 实际值显示 (带默认值)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(XActualDisplay))]
        private string _xActual = "1";

        public string XActualDisplay => $"{XActual}";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(YActualDisplay))]
        private string _yActual = "2";

        public string YActualDisplay => $"{YActual}";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ZActualDisplay))]
        private string _zActual = "3";

        public string ZActualDisplay => $"{ZActual}";

        // 冷却方式选择
        [ObservableProperty]
        private bool _isTimeCooling = true;

        [ObservableProperty]
        private bool _isTemperatureCooling;

        // 确保单选逻辑
        partial void OnIsTimeCoolingChanged(bool value)
        {
            if (value) IsTemperatureCooling = false;
        }

        partial void OnIsTemperatureCoolingChanged(bool value)
        {
            if (value) IsTimeCooling = false;
        }
    }
}
