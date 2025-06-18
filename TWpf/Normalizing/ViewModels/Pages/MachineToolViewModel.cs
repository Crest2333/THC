using Normalizing.Models;
using Normalizing.Siemens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.ViewModels.Pages
{
    public partial class MachineToolViewModel : ObservableObject
    {
        [ObservableProperty]
        private DeviceStatus status;
        public MachineToolViewModel(SiemensManager manager)
        {
            status = DeviceStatus.Normal;
            _manager = manager;
            xAxis = new Axis
            {
                Name = "X轴",
                Speed = 1,
                TargetSpeed = 1,
                ActualCurrent = 1,
                Location = 1,
                TargetLocation = 1
            };
            yAxis = new Axis
            {
                Name = "Y轴",
                Speed = 1,
                TargetSpeed = 1,
                ActualCurrent = 1,
                Location = 1,
                TargetLocation = 1
            };
            zAxis = new Axis
            {
                Name = "Z轴",
                Speed = 1,
                TargetSpeed = 1,
                ActualCurrent = 1,
                Location = 1,
                TargetLocation = 1
            };
        }
        public ObservableCollection<Axis> Axes { get; set; }
        [ObservableProperty]
        private decimal temperature1;
        private readonly SiemensManager _manager;

        [ObservableProperty]
        private Axis xAxis;
        [ObservableProperty]
        private Axis yAxis;
        [ObservableProperty]
        private Axis zAxis;

        [ObservableProperty]
        private TemperatureSensor temperatureSensor;
        [ObservableProperty]
        private FlowSensor flowSensor;
        [ObservableProperty]
        private DisplacementSensor displacementSensor;

        public MachineToolViewModel()
        {

        }

        [RelayCommand]
        public async Task GetAxisData()
        {
            var axisInfo = await _manager.ReadAxisInfoAsync();
            if (axisInfo != null)
            {
                XAxis = axisInfo.XAxis;
                YAxis = axisInfo.YAxis;
                ZAxis = axisInfo.ZAxis;
            }
            TemperatureSensor = await _manager.GetTemperatureSensorAsync();
            FlowSensor = await _manager.GetFlowSensorAsync();
            DisplacementSensor = await _manager.GetDisplacementSensorAsync();
        }
    }
}
