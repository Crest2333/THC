using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Modbus
{
    public class ModbusTcpMessage
    {
        /// <summary>
        /// 事务Id
        /// </summary>
        public ushort TransactionId { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public byte SlaveId { get; set; }

        /// <summary>
        /// 功能码
        /// </summary>
        public FunctionType FunctionCode { get; set; }

        /// <summary>
        /// 开始地址
        /// </summary>
        public int StartAddress { get; set; }

        /// <summary>
        /// 是否是线圈
        /// </summary>
        public bool IsCoil { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
