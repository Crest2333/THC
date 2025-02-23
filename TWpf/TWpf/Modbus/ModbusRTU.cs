using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RTU
{
    internal class ModbusRTU : IDisposable
    {
        private readonly SerialPort _serialPort;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName">端口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">校验</param>
        /// <param name="dataBits">数据位</param>
        public ModbusRTU(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;
            _serialPort.Open();
        }

        /// <summary>
        /// 读取输出线圈数据
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        public byte[] ReadOutputCoil(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            // 0x01 读取输出线圈
            byte[] request = BuildReadRequest(slaveId, 0x01, startAddress, numberOfPoints);

            _serialPort.Write(request, 0, request.Length);

            // 读取输出线圈报文格式
            // |从站地址|功能码|字节计数|                    字节                    |CRC  |
            // |1字节   |1字节 |1字节   |如果是8的倍数则除8，如果不是8的倍数则除8在+1|2字节|
            int expectedResponseLength = 5 + (numberOfPoints + 7) / 8; ;
            byte[] response = new byte[expectedResponseLength]; // 响应头(3字节) + 数据 + CRC(2字节)

            _serialPort.Read(response, 0, expectedResponseLength);
            // 验证CRC校验
            ushort receivedCrc = (ushort)((response[response.Length - 1] << 8) | response[response.Length - 2]);
            ushort calculatedCrc = CalculateCRC(response, response.Length - 2);

            if (receivedCrc != calculatedCrc)
            {
                throw new InvalidOperationException("CRC check failed.");
            }

            return response;

        }

        /// <summary>
        /// 读取输入线圈
        /// </summary>
        /// <param name="slavaId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] ReadInputCoil(byte slavaId, ushort startAddress, ushort numberOfPoints)
        {
            byte[] request = BuildReadRequest(slavaId, 0x02, startAddress, numberOfPoints);
            _serialPort.Write(request, 0, request.Length);

            // 读取输出线圈报文格式
            // |从站地址|功能码|字节计数|                    字节                    |CRC  |
            // |1字节   |1字节 |1字节   |如果是8的倍数则除8，如果不是8的倍数则除8在+1|2字节|
            int expectedResponseLength = 5 + (numberOfPoints + 7) / 8; ;
            byte[] response = new byte[expectedResponseLength]; // 响应头(3字节) + 数据 + CRC(2字节)

            _serialPort.Read(response, 0, expectedResponseLength);
            // 验证CRC校验
            ushort receivedCrc = (ushort)((response[response.Length - 1] << 8) | response[response.Length - 2]);
            ushort calculatedCrc = CalculateCRC(response, response.Length - 2);

            if (receivedCrc != calculatedCrc)
            {
                throw new InvalidOperationException("CRC check failed.");
            }

            return response;
        }


        /// <summary>
        /// 读取保持寄存器 
        /// 0x03 读取保持寄存器功能码
        /// </summary>
        /// <param name="slaveId">从站Id</param>
        /// <param name="startAddress">开始地址</param>
        /// <param name="numberOfPoints">读取寄存器数量</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            byte[] request = BuildReadRequest(slaveId, 3, startAddress, numberOfPoints);
            _serialPort.Write(request, 0, request.Length);

            // RTU 模式下 最小长度为4字节：从站地址（1）+功能码（1）+CRC校验码（2）
            // 有数据返回时 根据返回的数据量增加，读取N个寄存器，长度=5+2N字节
            // 5：从站地址（1）+功能码（1）+字节数（1）+CRC校验（2）
            // 2N：返回的N个寄存器数据的字节数
            int expectedResponseLength = 5 + 2 * numberOfPoints;
            byte[] response = new byte[expectedResponseLength];

            _serialPort.Read(response, 0, expectedResponseLength);

            // 验证CRC校验
            ushort receivedCrc = (ushort)((response[response.Length - 1] << 8) | response[response.Length - 2]);
            ushort calculatedCrc = CalculateCRC(response, response.Length - 2);

            if (receivedCrc != calculatedCrc)
            {
                throw new InvalidOperationException("CRC check failed.");
            }

            return response;
        }

        /// <summary>
        /// 读取保持型寄存器
        /// </summary>
        /// <param name="slavaId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] ReadInputRegisters(byte slavaId, ushort startAddress, ushort numberOfPoints)
        {
            byte[] request = BuildReadRequest(slavaId, 0x04, startAddress, numberOfPoints);
            _serialPort.Write(request, 0, request.Length);

            // RTU 模式下 最小长度为4字节：从站地址（1）+功能码（1）+CRC校验码（2）
            // 有数据返回时 根据返回的数据量增加，读取N个寄存器，长度=5+2N字节
            // 5：从站地址（1）+功能码（1）+字节数（1）+CRC校验（2）
            // 2N：返回的N个寄存器数据的字节数
            int expectedResponseLength = 5 + 2 * numberOfPoints;
            byte[] response = new byte[expectedResponseLength]; // 响应头(3字节) + 数据 + CRC(2字节)

            //_serialPort.Read(response, 0, expectedResponseLength);
            //// 验证CRC校验
            //ushort receivedCrc = (ushort)((response[response.Length - 1] << 8) | response[response.Length - 2]);
            //ushort calculatedCrc = CalculateCRC(response, response.Length - 2);

            //if (receivedCrc != calculatedCrc)
            //{
            //    throw new InvalidOperationException("CRC check failed.");
            //}

            return response;
        }

        /// <summary>
        /// 构建请求
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="functionCode"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        static byte[] BuildReadRequest(byte slaveId, byte functionCode, ushort startAddress, ushort numberOfPoints)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveId;
            frame[1] = functionCode;// 读取输出线圈功能码
            frame[2] = (byte)(startAddress >> 8); // 高位地址 将10进制右移8位，取10进制中的左边8位
            frame[3] = (byte)(startAddress & 0xFF); // 低位地址 和11111111取与操作，取10进制中的后8位
            frame[4] = (byte)(numberOfPoints >> 8); // 高位数量
            frame[5] = (byte)(numberOfPoints & 0xFF); // 低位数量

            ushort crc = CalculateCRC(frame, 6); // 计算CRC校验码
            frame[6] = (byte)(crc & 0xFF); // CRC低位
            frame[7] = (byte)(crc >> 8); // CRC高位

            return frame;
        }

        /// <summary>
        /// CRC计算
        /// </summary>
        /// <param name="request"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static ushort CalculateCRC(byte[] request, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                crc ^= request[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }

        public void Dispose()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
        }
    }
}
