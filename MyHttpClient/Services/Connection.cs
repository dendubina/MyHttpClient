using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MyHttpClientProject.Services.Interfaces;

namespace MyHttpClientProject.Services
{
    public class Connection : IConnection
    {
        private TcpClient _tcpClient;
        private string _connectionAddress;
        private ushort _connectionPort;

        private NetworkStream NetworkStream => _tcpClient != null ? _tcpClient.GetStream() : throw new InvalidOperationException("Not Connected");

        public async Task SendRequestAsync(string address, ushort port, IEnumerable<byte> data)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address must not be null or empty", nameof(address));
            }

            if (data == null)
            {
                throw new ArgumentException("Data must not be null", nameof(data));
            }

            if (!Connected(address, port))
            {
                OpenNewConnection(address, port);
            }

            var dataArray = data.ToArray();

            await NetworkStream.WriteAsync(dataArray, 0, dataArray.Length);
        }

        public IEnumerable<byte> ReadHeaders()
        {
            if (!NetworkStream.CanRead)
            {
                throw new InvalidOperationException("Stream is unable to read");
            }

            var sequenceToFind = Encoding.UTF8.GetBytes($"{Environment.NewLine}{Environment.NewLine}");
            var result = new List<byte>();

            bool endOfHeadersReached = false;

            while (!endOfHeadersReached)
            {
                var current = NetworkStream.ReadByte();

                result.Add((byte)current);

                endOfHeadersReached = result
                    .TakeLast(sequenceToFind.Length)
                    .SequenceEqual(sequenceToFind);
            }

            return result;
        }

        public async Task<IEnumerable<byte>> ReadBody(int bodyLength)
        {
            if (bodyLength <= 0)
            {
                throw new ArgumentException("Body length must be > 0");
            }

            if (!NetworkStream.CanRead)
            {
                throw new InvalidOperationException("Stream is unable to read");
            }

            var buffer = new byte[bodyLength];

            var bytesRead = await NetworkStream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead != bodyLength)
            {
                throw new InvalidOperationException("Invalid response body length");
            }

            return buffer;
        }

        private bool Connected(string address, ushort port)
            => _tcpClient is not {Connected: false} && _connectionAddress == address && _connectionPort == port;

        private void OpenNewConnection(string address, ushort port)
        {
            CloseConnection();

            _tcpClient = new TcpClient(address, port);

            _connectionAddress = address;
            _connectionPort = port;
        }

        public void CloseConnection() => _tcpClient?.Close();

        public void Dispose()
        {
            _tcpClient?.Close();
        }
    }
}

