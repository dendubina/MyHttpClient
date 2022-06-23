using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyHttpClientProject.WebConnection
{
    public class TcpClientConnection : IWebConnection
    {
        private TcpClient _tcpClient = new();
        private string _connectionAddress;
        private ushort _connectionPort;
        private const int ReadTimeout = 200;

        public async Task<byte[]> SendAsync(string address, ushort port, IEnumerable<byte> data)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address must not be null or empty", nameof(address));
            }

            if (!CheckConnection(address, port))
            {
                _tcpClient.Close();
                _tcpClient = new TcpClient(address, port);
                _connectionAddress = address;
                _connectionPort = port;
            }

            _tcpClient.Client.ReceiveTimeout = ReadTimeout;
            var networkStream = _tcpClient.GetStream();
            var dataArray = data.ToArray();
            

            await networkStream.WriteAsync(dataArray, 0, dataArray.Length);

            var buffer = new byte[_tcpClient.ReceiveBufferSize];

            //need to discuss code below
            var response = new MemoryStream();
            int bytesRead; 
            do
            {
                try
                {
                    bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    response.Write(buffer, 0, bytesRead);
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is not SocketException { ErrorCode: 10060 })
                        throw;

                    bytesRead = 0;
                }

            } while (bytesRead > 0);

            return response.ToArray();
        }

        public void Dispose() => _tcpClient.Close();

        private bool CheckConnection(string address, ushort port)
        {
            if (!_tcpClient.Connected)
            {
                return false;
            }

            return _connectionAddress == address && _connectionPort == port;
        }
    }
}
