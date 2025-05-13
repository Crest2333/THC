using RTU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Modbus
{
    public class ModbusRTUAdvance : ModbusRTU
    {
        /// <summary>
        /// 读取保持寄存器 
        /// 0x03 读取保持寄存器功能码
        /// </summary>
        /// <param name="slaveId">从站Id</param>
        /// <param name="startAddress">开始地址</param>
        /// <param name="numberOfPoints">读取寄存器数量</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ModbusMessage ReadRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            var data = Send(slaveId, FunctionType.ReadOutputRegister, startAddress, numberOfPoints);
            var span = data.AsSpan();
            var length = data[2];

            var result = new ModbusMessage
            {
                SlaveId = slaveId,
                Address = startAddress,
                Data = span.Slice(3, length).ToArray(),
                Type = ModbusMessageType.Register
            };
            return result;
        }
    }
}
