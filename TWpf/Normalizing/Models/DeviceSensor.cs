using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Models
{
    /// <summary>
    /// 设备传感器
    /// </summary>
    public class DeviceSensor
    {
        public int Id { get; set; }

        /// <summary>
        /// 传感器名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public decimal Value { get; set; }
    }
}
