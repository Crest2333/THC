using Normalizing.Models;
using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Siemens
{
    public class SiemensManager : IDisposable
    {
        public SiemensManager()
        {
            Plc = new Plc(CpuType.S71500, "192.168.0.12", 0, 1);
        }
        public Plc Plc { get; }

        public async Task SeedAsync()
        {
            if (!Plc.IsConnected)
            {
                await OpenAsync();
            }

            await Plc.WriteBytesAsync(DataType.DataBlock, 1, 1, [1, 2, 3, 4, 5, 6, 7]);
        }

        public async Task<AxisInfo> ReadAxisInfoAsync()
        {
            try
            {
                if (!Plc.IsConnected)
                {
                    await OpenAsync();
                }

                var data = (IEnumerable<float>)await Plc.ReadAsync(DataType.DataBlock, 1, 0, VarType.Real, 15);
                var result = new AxisInfo();
                var areaData = data.Chunk(5).ToArray();
                var xData = areaData[0];
                var yData = areaData[1];
                var zData = areaData[2];
                return new AxisInfo
                {
                    XAxis = CreateAxis("X轴", xData),
                    YAxis = CreateAxis("Y轴", yData),
                    ZAxis = CreateAxis("Z轴", zData)
                };
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task ReadSensorsAsync()
        {

        }

        private static Axis CreateAxis(string name, float[] xData)
        {
            return new Axis
            {
                Name = name,
                Location = xData[0],
                Speed = xData[1],
                TargetLocation = xData[2],
                TargetSpeed = xData[3],
                ActualCurrent = xData[4]
            };
        }

        public void Dispose()
        {
            if (Plc.IsConnected)
            {
                Plc.Close();
            }
        }

        public async Task<bool> OpenAsync()
        {
            try
            {
                await Plc.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal async Task<DisplacementSensor> GetDisplacementSensorAsync()
        {
            throw new NotImplementedException();
        }

        internal async Task<FlowSensor> GetFlowSensorAsync()
        {
            throw new NotImplementedException();
        }

        internal async Task<TemperatureSensor> GetTemperatureSensorAsync()
        {
            throw new NotImplementedException();
        }
    }
}
