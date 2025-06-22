using Normalizing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Services
{
    public class MockDeviceManager : IDeviceManager
    {
        // 线程安全的随机数生成器
        private static readonly Random _random = new Random();

        /// <summary>
        /// 生成指定范围的随机温度值（保留2位小数）
        /// </summary>
        private static float GenerateRandomTemperature(Random rand, float min, float max)
        {
            // 生成[0,1)区间的double，然后映射到指定范围
            double value = rand.NextDouble() * (max - min) + min;

            // 转换为float并保留两位小数
            return (float)Math.Round(value, 2);
        }

        /// <summary>
        /// 生成包含随机温度值的传感器对象
        /// </summary>
        /// <returns>随机初始化的TemperatureSensor实例</returns>
        private static TemperatureSensor GetRandomSensor()
        {
            // 使用线程安全的方式生成随机数（适用于多线程环境）
            int seed;
            lock (_random)
            {
                seed = _random.Next();
            }
            Random rand = new Random(seed);

            return new TemperatureSensor
            {
                // 生成范围示例：-40℃ ~ 150℃（可根据实际需求调整范围）
                RailHead = GenerateRandomTemperature(rand, -40, 150),
                RailFoot = GenerateRandomTemperature(rand, -40, 150),
                InnerRailFoot = GenerateRandomTemperature(rand, -40, 150),

                // 冷却温度范围示例：-20℃ ~ 80℃（假设冷却温度范围不同）
                Cooling = GenerateRandomTemperature(rand, -20, 80)
            };
        }

        private static FlowSensor GetRandomFlowSensor()
        {
            lock (_random)
            {
                return new FlowSensor
                {
                    RailHead = (float)Math.Round(_random.NextDouble() * 100, 2),
                    RailWeb = (float)Math.Round(_random.NextDouble() * 100, 2),
                    RailFoot = (float)Math.Round(_random.NextDouble() * 100, 2)
                };
            }
        }

        private static DisplacementSensor GetRandomDisplacementSensor()
        {
            lock (_random)
            {
                return new DisplacementSensor
                {
                    Y = (float)Math.Round(_random.NextDouble() * 100, 2),
                    X = (float)Math.Round(_random.NextDouble() * 100, 2),
                };
            }
        }

        /// <summary>
        /// 生成包含随机值的设备状态对象
        /// </summary>
        /// <returns>随机初始化的MachineStatus实例</returns>
        private static MachineStatus GetRandomStatus()
        {
            // 生成线程安全的随机种子
            int seed;
            lock (_random)
            {
                seed = _random.Next();
            }
            Random localRand = new Random(seed);

            return new MachineStatus
            {
                // 电气参数生成（带合理范围限制）
                Voltage = GenerateRandomFloat(localRand, 200f, 240f),   // 典型工业电压范围
                Current = GenerateRandomFloat(localRand, 0f, 50f),     // 典型设备电流范围
                Power = GenerateRandomFloat(localRand, 0f, 10000f),    // 功率范围（0-10kW）
                Energy = GenerateRandomFloat(localRand, 0f, 1000000f), // 能量范围（0-1MJ）

                // 设备状态生成（包含多种可能状态）
                IgbmPowerState = GenerateRandomStatus(localRand),
                YAxisTrackingState = GenerateRandomStatus(localRand),
                ZAxisTrackingState = GenerateRandomStatus(localRand),
                TopCylinderSafetyState = GenerateRandomStatus(localRand),
                ProcessingAllowedState = GenerateRandomStatus(localRand),
                FeedingCompletedState = GenerateRandomStatus(localRand),
                AutoRunState = GenerateRandomStatus(localRand)
            };
        }

        /// <summary>
        /// 生成指定范围的随机浮点数（保留2位小数）
        /// </summary>
        private static float GenerateRandomFloat(Random rand, float min, float max)
        {
            double value = rand.NextDouble() * (max - min) + min;
            return (float)Math.Round(value, 2);
        }

        /// <summary>
        /// 生成随机设备状态
        /// </summary>
        private static DeviceStatus GenerateRandomStatus(Random rand)
        {
            Array values = Enum.GetValues(typeof(DeviceStatus));
            return (DeviceStatus)values.GetValue(rand.Next(values.Length));
        }
        public Task<AxisInfo> ReadAxisInfoAsync()
        {
            return Task.FromResult(new AxisInfo());
        }

        public Task<DisplacementSensor> ReadDisplacementSensorAsync()
        {
            return Task.FromResult(GetRandomDisplacementSensor());
        }

        public Task<FlowSensor> ReadFlowSensorAsync()
        {
            return Task.FromResult(GetRandomFlowSensor());
        }

        public Task<TemperatureSensor> ReadTemperatureSensorAsync()
        {
            return Task.FromResult(GetRandomSensor());
        }

        public Task<MachineStatus> ReadMachineStatusAsync()
        {
            return Task.FromResult(GetRandomStatus());
        }
    }
}
