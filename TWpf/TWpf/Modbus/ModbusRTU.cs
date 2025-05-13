using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TWpf.Modbus;

namespace RTU
{
    public class ModbusRTU : IDisposable
    {
        private readonly SerialPort _serialPort;

        public int ReadTimeout { get; set; } = 1000;

        public int WriteTimeout { get; set; } = 1000;

        private int ByteTimeout { get; set; }

        private int Timeout { get; set; } = 1000;

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
            _serialPort.ReadTimeout = ReadTimeout;
            _serialPort.WriteTimeout = WriteTimeout;
        }

        public ModbusRTU()
        {
            _serialPort = new SerialPort();
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        public bool Connection(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (_serialPort.IsOpen)
            {
                return true;
            }


            _serialPort.ReadTimeout = ReadTimeout;
            _serialPort.WriteTimeout = WriteTimeout;
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.Parity = parity;
            _serialPort.DataBits = dataBits;
            _serialPort.StopBits = stopBits;
            try
            {
                _serialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Open()
        {
            if (_serialPort.IsOpen)
            {
                return;
            }

            _serialPort.Open();
        }

        public bool Close()
        {
            if (!_serialPort.IsOpen)
            {
                return true;
            }
            _serialPort.Close();
            return true;
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
            return Send(slaveId, FunctionType.ReadOutputCoil, startAddress, numberOfPoints);
        }

        /// <summary>
        /// 读取输入线圈
        /// </summary>
        /// <param name="slavaId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] ReadInputCoil(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            return Send(slaveId, FunctionType.ReadInputCoil, startAddress, numberOfPoints);

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
            return Send(slaveId, FunctionType.ReadOutputRegister, startAddress, numberOfPoints);
        }

        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        /// <param name="slavaId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] ReadInputRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            return Send(slaveId, FunctionType.ReadInputRegister, startAddress, numberOfPoints);
        }

        public byte[] Send(byte slaveId, FunctionType functionType, ushort startAddress, ushort numberOfPoints)
        {
            var request = BuildReadRequest(slaveId, functionType, startAddress, numberOfPoints);
            int expectedResponseLength = functionType is FunctionType.ReadInputCoil or FunctionType.ReadOutputCoil ? 5 + (numberOfPoints + 7) / 8 : 5 + 2 * numberOfPoints;

            return Send(request, expectedResponseLength);
        }

        public byte[] Send(byte[] request, int expectedResponseLength)
        {
            _serialPort.Write(request, 0, request.Length);

            // 读取线圈报文格式
            // |从站地址|功能码|字节计数|                    字节                    |CRC  |
            // |1字节   |1字节 |1字节   |如果是8的倍数则除8，如果不是8的倍数则除8在+1|2字节|
            // 对于寄存器报文格式
            // |从站地址|功能码|字节计数|                    字节                    |CRC  |
            // |1字节   |1字节 |1字节   |           2X读取寄存器数量                 |2字节|

            byte[] response = new byte[expectedResponseLength]; // 响应头(3字节) + 数据 + CRC(2字节)
            WaitMore(_serialPort, 1 + 1 + 2);
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

        public byte[] WriteSingleCoil(byte slaveId, ushort coilAddress, bool coilValue)
        {
            byte[] bytes = new byte[2];
            bytes[0] = coilValue ? (byte)0xFF : (byte)0x00;
            bytes[1] = (byte)0x00;
            var request = BuildWriteRequest(slaveId, FunctionType.WriteSingleCoil, coilAddress, bytes);
            return Send(request, request.Length);

        }

        public byte[] WriteSingleRegister(byte slaveId, ushort registerAddress, ushort registerValue)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(registerValue >> 8); // 高位数量
            bytes[1] = (byte)(registerValue & 0xFF); // 低位数量
            var request = BuildWriteRequest(slaveId, FunctionType.WriteSingleRegister, registerAddress, bytes);
            return Send(request, request.Length);
        }

        private void WaitMore(SerialPort sp, int minLength)
        {
            var count = sp.BytesToRead;
            if (count >= minLength) return;

            var ms = ByteTimeout > 0 ? ByteTimeout : 10;
            var sw = Stopwatch.StartNew();
            while (sp.IsOpen && sw.ElapsedMilliseconds < Timeout)
            {
                //Thread.SpinWait(1);
                Thread.Sleep(ms);
                if (count != sp.BytesToRead)
                {
                    count = sp.BytesToRead;
                    if (count >= minLength) break;

                    //sw.Restart();
                }
            }
        }

        private byte[] WriteMultipleRegisters(byte slaveId, ushort startingAddress, byte[] registerValues)
        {
            var request = BuildMultipleWriteRequest(slaveId, FunctionType.WriteMultipleRegister, startingAddress, registerValues);
            return Send(request, request.Length);
        }


        /// <summary>
        /// 构建请求
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="functionCode"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        static byte[] BuildReadRequest(byte slaveId, FunctionType functionType, ushort startAddress, ushort numberOfPoints)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveId;
            frame[1] = (byte)functionType;// 读取输出线圈功能码
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
        /// 构建请求
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="functionCode"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        static byte[] BuildWriteRequest(byte slaveId, FunctionType functionType, ushort startAddress, byte[] writeValue)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveId;
            frame[1] = (byte)functionType;// 线圈功能码
            frame[2] = (byte)(startAddress >> 8); // 高位地址 将10进制右移8位，取10进制中的左边8位
            frame[3] = (byte)(startAddress & 0xFF); // 低位地址 和11111111取与操作，取10进制中的后8位
            frame[4] = (byte)(writeValue[0]); // 高位数量
            frame[5] = (byte)(writeValue[1]); // 低位数量

            ushort crc = CalculateCRC(frame, 6); // 计算CRC校验码
            frame[6] = (byte)(crc & 0xFF); // CRC低位
            frame[7] = (byte)(crc >> 8); // CRC高位

            return frame;
        }

        /// <summary>
        /// 构建请求
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="functionCode"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        static byte[] BuildMultipleWriteRequest(byte slaveId, FunctionType functionType, ushort startAddress, byte[] writeValue)
        {
            int registerCount = writeValue.Length;
            int byteCount = registerCount * 2;      // 每个寄存器2个字节
            int requestLength = 9 + byteCount;      // 固定部分长度 + 数据长度

            byte[] frame = new byte[requestLength];
            frame[0] = slaveId;
            frame[1] = (byte)functionType;// 线圈功能码
            frame[2] = (byte)(startAddress >> 8); // 高位地址 将10进制右移8位，取10进制中的左边8位
            frame[3] = (byte)(startAddress & 0xFF); // 低位地址 和11111111取与操作，取10进制中的后8位
            frame[4] = (byte)(registerCount >> 8);  // 寄存器数量高字节
            frame[5] = (byte)registerCount;         // 寄存器数量低字节
            frame[6] = (byte)byteCount;             // 字节数

            // 填充寄存器数据
            for (int i = 0; i < registerCount; i++)
            {
                frame[7 + i * 2] = (byte)(writeValue[i] >> 8);      // 高字节
                frame[7 + i * 2 + 1] = (byte)writeValue[i];          // 低字节
            }

            ushort crc = CalculateCRC(frame, frame.Length - 2); // 计算CRC校验码
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
