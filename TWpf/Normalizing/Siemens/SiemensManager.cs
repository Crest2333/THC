using Normalizing.Models;
using Normalizing.Services;
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
    public class SiemensManager : IDeviceManager, IDisposable
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
            if (Plc.IsConnected)
            {
                return true;
            }
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

        public async Task<DisplacementSensor> ReadDisplacementSensorAsync()
        {
            if (!await OpenAsync())
            {
                return new DisplacementSensor();
            }
            try
            {
                var result = new DisplacementSensor();
                var data = (float[])await Plc.ReadAsync(DataType.DataBlock, 1, 4 * 22, VarType.Real, 3);
                result.X = data[0];
                result.Y = data[1];
                return result;
            }
            catch (Exception ex)
            {
                return new DisplacementSensor();
            }
        }

        public async Task<FlowSensor> ReadFlowSensorAsync()
        {
            if (!await OpenAsync())
            {
                return new FlowSensor();
            }
            try
            {
                var result = new FlowSensor();
                var data = (float[])await Plc.ReadAsync(DataType.DataBlock, 1, 4 * 19, VarType.Real, 3);
                result.RailHead = data[0];
                result.RailWeb = data[1];
                result.RailFoot = data[2];

                return result;
            }
            catch (Exception ex)
            {
                return new FlowSensor();
            }
        }

        public async Task<TemperatureSensor> ReadTemperatureSensorAsync()
        {
            if (!await OpenAsync())
            {
                return new TemperatureSensor();
            }
            try
            {
                var result = new TemperatureSensor();
                var data = (float[])await Plc.ReadAsync(DataType.DataBlock, 1, 4 * 15 , VarType.Real, 4);
                result.InnerRailFoot = data[0];
                result.RailFoot = data[1];
                result.Cooling = data[2];
                result.RailHead = data[3];
                return result;
            }
            catch(Exception ex)
            {
                return new TemperatureSensor();
            }
        }

        public async Task<MachineStatus> ReadMachineStatusAsync()
        {
            if (!await OpenAsync())
            {
                return new MachineStatus();
            }
            try
            {
                var result = new MachineStatus();
                var data = (float[])await Plc.ReadAsync(DataType.DataBlock, 1, 4 * 24, VarType.Real, 4);
                result.Voltage = data[0];
                result.Current = data[1];
                result.Power = data[2];
                result.Energy = data[3];
                var stateData = (short[])await Plc.ReadAsync(DataType.DataBlock, 1, 4 * 28, VarType.Int, 7);

                result.IgbmPowerState = (DeviceStatus)stateData[0];
                result.YAxisTrackingState = (DeviceStatus)stateData[1];
                result.ZAxisTrackingState = (DeviceStatus)stateData[2];
                result.TopCylinderSafetyState = (DeviceStatus)stateData[3];
                result.ProcessingAllowedState = (DeviceStatus)stateData[4];
                result.FeedingCompletedState = (DeviceStatus)stateData[5];
                result.AutoRunState = (DeviceStatus)stateData[6];

                return result;
            }
            catch (Exception ex)
            {
                return new MachineStatus();
            }
        }
    }
}
