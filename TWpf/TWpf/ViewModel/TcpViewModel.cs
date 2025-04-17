using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TWpf.ViewModel
{
    internal partial class TcpViewModel : ObservableObject
    {

        [ObservableProperty]
        private string ipAddress;
        [ObservableProperty]
        private int port;
        [ObservableProperty]
        private ObservableCollection<TcpMessage> receiveMessages;
        [ObservableProperty]
        private ObservableCollection<string> clients;
        [ObservableProperty]
        private bool isStart;
        [ObservableProperty]
        private bool isClose;
        [ObservableProperty]
        private string data;

        private readonly Dispatcher _dispatcher;
        private Socket socket;
        private ConcurrentDictionary<string, Socket> sockets;
        private CancellationTokenSource _cts; // 添加字段

        public TcpViewModel(Dispatcher dispatcher)
        {
            sockets = new ConcurrentDictionary<string, Socket>();
            _dispatcher = dispatcher;
            receiveMessages = new ObservableCollection<TcpMessage>();
            clients = new ObservableCollection<string>();
            isStart = false;
            isClose = true;
            InitLocalIpAddress();
        }

        private void InitLocalIpAddress()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface nic in interfaces)
                {
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                    (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        IPInterfaceProperties ipProps = nic.GetIPProperties();
                        foreach (UnicastIPAddressInformation ipInfo in ipProps.UnicastAddresses)
                        {
                            if (ipInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                IpAddress = ipInfo.Address.ToString();
                                port = 20;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("本机地址查询异常");
            }
        }

        [RelayCommand]
        private void Start()
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                MessageBox.Show("ip地址必填");
                return;
            }
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (IPAddress.TryParse(ipAddress, out var address))
            {
                var ipe = new IPEndPoint(address, port);
                socket.Bind(ipe);
                socket.Listen();
            }
            ProgressAsync();
            IsStart = true;
            IsClose = false;
        }

        [RelayCommand]
        private void Close()
        {
            _cts.Cancel();
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
            // 关闭所有客户端连接
            foreach (var client in sockets.Values)
            {
                client.Close();
            }
            sockets.Clear();
            _dispatcher.InvokeAsync(() => clients.Clear());
            IsStart = false;
            IsClose = true;
        }

        [RelayCommand]
        private async void Send()
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            var bt = Encoding.UTF8.GetBytes(data);
            foreach (var clientSocket in sockets.Values)
            {
                try
                {
                    await clientSocket.SendAsync(bt);
                }
                catch (Exception ex)
                {
                    // 处理发送异常，例如客户端断开连接
                    // 可以在这里移除断开的客户端
                }
            }
            await _dispatcher.InvokeAsync(() =>
             {
                 receiveMessages.Add(new TcpMessage
                 {
                     ClientId = "server",
                     Message = data
                 });
             });
        }

        private async Task ProgressAsync()
        {
            _cts = new CancellationTokenSource();
            while (true)
            {
                if (_cts.IsCancellationRequested)
                {
                    return;
                }
                var client = await socket.AcceptAsync();
                var clientId = client.RemoteEndPoint.ToString();
                sockets.TryAdd(clientId, client);
                await _dispatcher.InvokeAsync(() =>
                {
                    clients.Add(clientId);
                });
                await ReceiveAsync(clientId, client);
            }
        }

        private async Task ReceiveAsync(string clientId, Socket client)
        {
            var buffer = new byte[1024 * 1024];

            while (true)
            {
                var bytesRead = await client.ReceiveAsync(buffer);
                if (bytesRead == 0)
                {
                    // 当读取到 0 字节时，表明客户端已断开连接
                    sockets.TryRemove(clientId, out var remove);
                    await _dispatcher.InvokeAsync(() =>
                    {
                        clients.Remove(clientId);
                    });
                    break;
                }
                var message = Encoding.UTF8.GetString(buffer);
                await _dispatcher.InvokeAsync(() =>
                  {
                      ReceiveMessages.Add(new TcpMessage
                      {
                          ClientId = clientId,
                          Message = message
                      });
                  });
            }
        }
    }

    class TcpMessage
    {
        public string ClientId { get; set; }

        public string Message { get; set; }
    }
}
