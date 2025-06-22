using Normalizing.Models;
using Normalizing.Services;
using Normalizing.Siemens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.ViewModels.Pages
{
    public partial class MachineToolViewModel : ObservableObject, IDisposable
    {
        public MachineToolViewModel(IDeviceManager manager)
        {

            _manager = manager;
            xAxis = new Axis
            {
                Name = "X轴",

            };
            yAxis = new Axis
            {
                Name = "Y轴",

            };
            zAxis = new Axis
            {
                Name = "Z轴",

            };
            Axes = new ObservableCollection<Axis>
            {
                XAxis,
                YAxis,
                ZAxis,
            };
            ReadData();
        }
        [ObservableProperty]
        private ObservableCollection<Axis> axes;
        [ObservableProperty]
        private decimal temperature1;
        private readonly IDeviceManager _manager;

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
        [ObservableProperty]
        private MachineStatus machineStatus;

        public async Task ReadData()
        {
            while (true)
            {
                await Task.Delay(3000);

                var axisInfo = await _manager.ReadAxisInfoAsync();

                if (axisInfo != null)
                {
                    Axes = new ObservableCollection<Axis>
                    {
                        axisInfo.XAxis,
                        axisInfo.YAxis,
                        axisInfo.ZAxis
                    };
                    XAxis = axisInfo.XAxis;
                    YAxis = axisInfo.YAxis;
                    ZAxis = axisInfo.ZAxis;
                }
                TemperatureSensor = await _manager.ReadTemperatureSensorAsync();
                FlowSensor = await _manager.ReadFlowSensorAsync();
                DisplacementSensor = await _manager.ReadDisplacementSensorAsync();
                MachineStatus = await _manager.ReadMachineStatusAsync();

            }

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
