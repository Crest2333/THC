using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Common
{
    public static class SensorDataGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// 生成随机的温度和湿度数据并编码为字节数组
        /// </summary>
        /// <returns>字节数组：[温度高字节, 温度低字节, 湿度高字节, 湿度低字节]</returns>
        public static byte[] GenerateRandomSensorData(
           )
        {
            double minTemp = -40;
            double maxTemp = 85;
            double minHumidity = 0;
            double maxHumidity = 100;
            // 生成随机温度值 (-40℃ 到 85℃)，精度0.1℃
            double temperature = Math.Round(_random.NextDouble() * (maxTemp - minTemp) + minTemp, 1);

            // 生成随机湿度值 (0% 到 100%)，精度0.1%
            double humidity = Math.Round(_random.NextDouble() * (maxHumidity - minHumidity) + minHumidity, 1);

            // 温度值放大10倍并转为整数 (例如：25.5℃ → 255)
            short tempValue = (short)(temperature * 10);

            // 湿度值放大10倍并转为整数 (例如：60.2% → 602)
            short humidityValue = (short)(humidity * 10);

            // 转换为字节数组
            return new byte[]
            {
            (byte)(tempValue >> 8),  // 温度高字节
            (byte)(tempValue & 0xFF), // 温度低字节
            (byte)(humidityValue >> 8),  // 湿度高字节
            (byte)(humidityValue & 0xFF)  // 湿度低字节
            };
        }

        /// <summary>
        /// 从字节数组解析温度值
        /// </summary>
        public static decimal ParseTemperature(byte[] data)
        {
            short tempValue = (short)((data[0] << 8) | data[1]);
            return Math.Round(tempValue / 10.0M, 2);
        }

        /// <summary>
        /// 从字节数组解析湿度值
        /// </summary>
        public static decimal ParseHumidity(byte[] data)
        {
            short humidityValue = (short)((data[2] << 8) | data[3]);
            return Math.Round(humidityValue / 10.0M, 2);
        }
    }
}
