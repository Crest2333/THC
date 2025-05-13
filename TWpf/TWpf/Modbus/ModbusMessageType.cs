using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Modbus
{
    public enum ModbusMessageType
    {
        /// <summary>
        ///  线圈
        /// </summary>
        Coil,

        /// <summary>
        /// 寄存器
        /// </summary>
        Register
    }
}
