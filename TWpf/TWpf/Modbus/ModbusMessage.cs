using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Modbus
{
    public class ModbusMessage
    {
        /// <summary>
        /// 从站Id
        /// </summary>
        public int SlaveId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public ModbusMessageType Type { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
