using System;
using MyHttpClientProject.Services.Interfaces;
using System.IO;
using System.Net.Sockets;

namespace MyHttpClientProject.Services
{
    public class MyTcpClient : IClient
    {
        private TcpClient _tcpClient;
        private string _connectionAddress;
        private ushort _connectionPort;

        public Stream GetStream()
            => _tcpClient != null ? _tcpClient.GetStream() : throw new InvalidOperationException("Not Connected");

        public bool IsConnected(string address, ushort port)
            => _tcpClient is not { Connected: false } && _connectionAddress == address && _connectionPort == port;

        public void OpenConnection(string address, ushort port, int sendTimeout, int receiveTimeout)
        {
            CloseConnection();

            _tcpClient = new TcpClient(address, port);

            _tcpClient.Client.ReceiveTimeout = receiveTimeout;
            _tcpClient.Client.SendTimeout = sendTimeout;

            _connectionAddress = address;
            _connectionPort = port;
        }

        public void CloseConnection() => _tcpClient?.Close();
    }
}
