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
        public MachineToolViewModel(SiemensManager manager)
        {
            _manager = manager;
        }
        public ObservableCollection<Axis> Axes { get; set; }
        [ObservableProperty]
        private decimal temperature1;
        private readonly SiemensManager _manager;

        private readonly Axis xAxis;
        private readonly Axis yAxis;
        private readonly Axis zAxis;
        public MachineToolViewModel()
        {
            xAxis = new Axis
            {
                Name = "X轴"
            };
            yAxis = new Axis
            {
                Name = "Y轴"
            };
            zAxis = new Axis
            {
                Name = "Z轴"
            };
            Axes = new ObservableCollection<Axis>
            {
               xAxis,
               yAxis,
               zAxis
            };
        }

        [RelayCommand]
        public async Task GetAxisData()
        {
            await _manager.OpenAsync();

            var res = await _manager.Plc.ReadBytesAsync(S7.Net.DataType.DataBlock, 1, 2, 12);

        }
    }
}
