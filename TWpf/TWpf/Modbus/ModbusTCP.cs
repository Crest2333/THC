using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Modbus
{
    internal class ModbusTCP
    {
        private readonly Socket _socket;
        public ModbusTCP()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task ConnectAsync(string host, int port)
        {
            if (_socket.Connected)
            {
                return;
            }
            await _socket.ConnectAsync(host, port);
        }

        public void Close()
        {
            if (!_socket.Connected)
            {
                return;   
            }
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

    }
}
