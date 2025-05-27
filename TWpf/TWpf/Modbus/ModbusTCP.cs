using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace TWpf.Modbus
{
    internal class ModbusTCP
    {
        private readonly Socket _socket;
        private int _transactionId;

        public ModbusTCP()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task ConnectAsync(string ipAddress, int port, int timeout)
        {
            if (_socket.Connected)
            {
                return;
            }
            _socket.ReceiveTimeout = timeout;
            _socket.SendTimeout = timeout;
            await _socket.ConnectAsync(ipAddress, port);
        }

        public async Task<ModbusTcpMessage> ReadRegisterAsync(byte salveId, FunctionType functionType, ushort startAddress, ushort length)
        {
            var data = BuildReadRequest(salveId, functionType, startAddress, length);

            await _socket.SendAsync(data);

            var result = new byte[1024];
            var bytesRead = await _socket.ReceiveAsync(result);
            return ParseReadResponse(result, startAddress, bytesRead);

        }

        public async Task<ModbusTcpMessage> WriteRegisterAsync(byte salveId, FunctionType functionType, ushort startAddress, ushort value)
        {
            var data = BuildWriteRequest(salveId, functionType, startAddress, value);

            await _socket.SendAsync(data);

            var result = new byte[1024];
            var bytesRead = await _socket.ReceiveAsync(result);
            return ParseWriteResponse(result, startAddress, bytesRead);
        }

        public async Task<ModbusTcpMessage> WriteMultipleRegisterAsync(byte salveId, FunctionType functionType, ushort startAddress, ushort number, ushort[] values)
        {
            var data = BuildMultipleWriteRequest(salveId, functionType, startAddress, number, values);

            await _socket.SendAsync(data);

            var result = new byte[1024];
            var bytesRead = await _socket.ReceiveAsync(result);
            return ParseWriteResponse(result, startAddress, bytesRead);
        }

        private byte[] BuildReadRequest(byte salveId, FunctionType functionType, ushort startAddress, ushort length)
        {
            var request = new byte[12];
            Interlocked.Increment(ref _transactionId);
            // MBAP 
            request[0] = (byte)(_transactionId >> 8);
            request[1] = (byte)(_transactionId & 0xFF);
            request[2] = 0;
            request[3] = 0;
            request[4] = 0;
            request[5] = 6;
            request[6] = salveId;

            request[7] = (byte)functionType;
            request[8] = (byte)(startAddress >> 8);
            request[9] = (byte)(startAddress & 0xFF);
            request[10] = (byte)(length >> 8);
            request[11] = (byte)(length & 0xFF);
            return request;
        }

        private ModbusTcpMessage ParseReadResponse(byte[] buffer, int startAddress, int length)
        {
            // 检查响应长度是否合法
            if (length < 9)
                throw new InvalidDataException("Modbus响应长度不足");

            // 解析MBAP头
            ushort responseTransactionId = (ushort)((buffer[0] << 8) | buffer[1]);
            ushort protocolId = (ushort)((buffer[2] << 8) | buffer[3]);
            ushort responseLength = (ushort)((buffer[4] << 8) | buffer[5]);
            byte slaveId = buffer[6];
            byte functionCode = buffer[7];

            // 检查事务ID和功能码
            if (responseTransactionId != _transactionId)
                throw new InvalidDataException("事务ID不匹配");

            // 检查是否为异常响应
            if ((functionCode & 0x80) == 0x80)
            {
                byte exceptionCode = buffer[8];
                throw new InvalidDataException("错误响应");
            }

            // 提取数据部分
            byte[] data = new byte[length - 9];
            Array.Copy(buffer, 9, data, 0, data.Length);

            return new ModbusTcpMessage
            {
                TransactionId = responseTransactionId,
                StartAddress = startAddress,
                SlaveId = slaveId,
                FunctionCode = (FunctionType)functionCode,
                Data = data
            };
        }

        private ModbusTcpMessage ParseWriteResponse(byte[] buffer, int startAddress, int length)
        {
            // 解析MBAP头
            ushort responseTransactionId = (ushort)((buffer[0] << 8) | buffer[1]);
            ushort protocolId = (ushort)((buffer[2] << 8) | buffer[3]);
            ushort responseLength = (ushort)((buffer[4] << 8) | buffer[5]);
            byte salveId = buffer[6];
            byte functionCode = buffer[7];

            // 检查事务ID和功能码
            if (responseTransactionId != _transactionId)
                throw new InvalidDataException("事务ID不匹配");

            // 检查是否为异常响应
            if ((functionCode & 0x90) == 0x90)
            {
                byte exceptionCode = buffer[8];
                throw new InvalidDataException("错误响应");
            }
            // 验证起始地址
            ushort responseStartAddress = (ushort)((buffer[8] << 8) | buffer[9]);
            if (responseStartAddress != startAddress)
                throw new InvalidDataException("起始地址不匹配");

            // 提取数据部分
            byte[] data = new byte[length - 9];
            Array.Copy(buffer, 9, data, 0, data.Length);

            return new ModbusTcpMessage
            {
                TransactionId = responseTransactionId,
                FunctionCode = (FunctionType)functionCode,
                Data = data
            };
        }

        private byte[] BuildWriteRequest(byte salveId, FunctionType functionType, ushort startAddress, ushort value)
        {
            byte[] data = new byte[12];
            Interlocked.Increment(ref _transactionId);
            byte[] _id = BitConverter.GetBytes((short)_transactionId);
            data[0] = _id[1];
            data[1] = _id[0];
            //协议标识符号
            data[2] = 0;
            data[3] = 0;
            //长度
            byte[] _size = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)(8)));
            data[4] = _size[0];
            data[5] = _size[1];
            //从站Id
            data[6] = salveId;
            data[7] = (byte)functionType;
            //地址
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            data[8] = _adr[0];
            data[9] = _adr[1];
            //数据
            byte[] _cnt = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)value));
            data[10] = _cnt[0];
            data[11] = _cnt[1];

            return data;
        }

        private byte[] BuildMultipleWriteRequest(byte salveId, FunctionType functionType, ushort startAddress, ushort number, ushort[] values)
        {
            var valueLength = values.Length * 2;// *2一个value占2个字节

            int totalLength = 6 /* MBAP */ + 1 /* Unit ID */ + 1 /* Function Code */ + 2 /* Address */ + 2 /* Quantity */ + 1 /* Byte Count */ + valueLength;
            byte[] data = new byte[totalLength];
            Interlocked.Increment(ref _transactionId);
            byte[] _id = BitConverter.GetBytes((short)_transactionId);
            data[0] = _id[1];
            data[1] = _id[0];
            //协议标识符号
            data[2] = 0;
            data[3] = 0;
            //长度
            byte[] _size = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)(8)));
            data[4] = _size[0];
            data[5] = _size[1];
            //从站Id
            data[6] = salveId;
            data[7] = (byte)functionType;
            //地址
            byte[] addressBytes = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            data[8] = addressBytes[0];
            data[9] = addressBytes[1];

            //寄存器数量
            byte[] numberBytes = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)number));
            data[10] = numberBytes[0];
            data[11] = numberBytes[1];

            data[12] = (byte)valueLength;
            //数据
            for (int i = 0; i < values.Length; i++)
            {
                byte[] valueByte = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)values[i]));

                data[13 + i * 2] = valueByte[0];
                data[14 + i * 2] = valueByte[1];
            }
            return data;
        }
    }
}
