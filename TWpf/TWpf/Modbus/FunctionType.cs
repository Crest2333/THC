using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Modbus
{
    public enum FunctionType : byte
    {
        /// <summary>
        /// 读取输入线圈
        /// </summary>
        ReadInputCoil = 1,

        /// <summary>
        /// 读取输出线圈
        /// </summary>
        ReadOutputCoil = 2,

        /// <summary>
        /// 读取寄存器
        /// </summary>
        ReadOutputRegister = 3,

        /// <summary>
        /// 读取输出寄存器
        /// </summary>
        ReadInputRegister = 4,

        /// <summary>
        /// 写单个线圈
        /// </summary>
        WriteSingleCoil = 5,

        /// <summary>
        /// 写单个寄存器
        /// </summary>
        WriteSingleRegister = 6,

        /// <summary>
        /// 写多个线圈
        /// </summary>
        WriteMultipleCoil = 15,

        /// <summary>
        /// 写多个寄存器
        /// </summary>
        WriteMultipleRegister = 16,
    }
}
