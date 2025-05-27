using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Modbus
{
    public class DeviceData
    {
        /// <summary>
        /// 地址
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public int Value { get; set; }
    }
}
