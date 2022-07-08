using System;
using System.IO;
using System.Net.Sockets;
using MyHttpClientProject.Services.Interfaces;

namespace MyHttpClientProject.Services
{
    public class MyTcpClient : ITcpClient
    {
        private TcpClient _tcpClient;
        private string _connectionAddress;
        private ushort _connectionPort;

        public void OpenNewConnection(string address, ushort port)
        {
            Close();

            _tcpClient = new TcpClient(address, port);

            _connectionAddress = address;
            _connectionPort = port;
        }

        public Stream GetStream()
            => _tcpClient != null ? _tcpClient.GetStream() : throw new InvalidOperationException("Not connected");

        public bool Connected(string address, ushort port)
        {
            if (_tcpClient is not { Connected: true })
            {
                return false;
            }

            return _connectionAddress == address && _connectionPort == port;
        }

        public void Close() => _tcpClient?.Close();

        public void Dispose() => Close();
    }
}
