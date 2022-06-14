using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyHttpClientProject.WebConnection
{
    public class TcpClientConnection : IWebConnection
    {
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private string _connectionAddress;
        private ushort _connectionPort;
        private const int ReadTimeout = 200;

        public async Task<byte[]> Send(string address, ushort port, IEnumerable<byte> data)
        {
            if (!CheckIfConnected(address, port))
            {
                _tcpClient = new TcpClient(address, port);
                _networkStream = _tcpClient.GetStream();
                _connectionAddress = address;
                _connectionPort = port;
            }

            _tcpClient.Client.ReceiveTimeout = ReadTimeout;
            var dataArray = data.ToArray();
            var response = new MemoryStream();

            await _networkStream.WriteAsync(dataArray, 0, dataArray.Length);

            var buffer = new byte[_tcpClient.ReceiveBufferSize];

            //need to discuss code below
            int bytesRead; 
            do
            {
                try
                {
                    bytesRead = _networkStream.Read(buffer, 0, buffer.Length);
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

        public void Dispose()
        {
            _networkStream.Close();
            _tcpClient.Close();
        }

        private bool CheckIfConnected(string address, ushort port)
        {
            if (_tcpClient == null)
            {
                return false;
            }

            if (!_tcpClient.Connected)
            {
                return false;
            }

            if (_connectionAddress == address && _connectionPort == port)
            {
                return true;
            }

            Dispose();
            return false;
        }
    }
}
