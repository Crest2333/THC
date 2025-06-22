using Normalizing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Services
{
    public interface IDeviceManager
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        Task<DisplacementSensor> ReadDisplacementSensorAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<FlowSensor> ReadFlowSensorAsync();

        /// <summary>
        /// 温度传感器数据
        /// </summary>
        /// <returns></returns>
        Task<TemperatureSensor> ReadTemperatureSensorAsync();

        /// <summary>
        /// 获取轴信息
        /// </summary>
        /// <returns></returns>
        Task<AxisInfo> ReadAxisInfoAsync();

        /// <summary>
        /// 设备信息
        /// </summary>
        /// <returns></returns>
        Task<MachineStatus> ReadMachineStatusAsync();
    }
}
