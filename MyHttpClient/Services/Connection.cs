using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHttpClientProject.Services.Interfaces;

namespace MyHttpClientProject.Services
{
    public class Connection : IConnection
    {
        private readonly ITcpClient _tcpClient;

        public Connection() : this(new MyTcpClient())
        {

        }

        public Connection(ITcpClient client)
        {
            _tcpClient = client;
        }

        public async Task SendAsync(string address, ushort port, IEnumerable<byte> data)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address must not be null or empty", nameof(address));
            }

            if (data == null)
            {
                throw new ArgumentException("Data must not be null", nameof(data));
            }

            if (!_tcpClient.Connected(address, port))
            {
                _tcpClient.OpenNewConnection(address, port);
            }

            var networkStream = _tcpClient.GetStream();
            var dataArray = data.ToArray();

            await networkStream.WriteAsync(dataArray, 0, dataArray.Length);
        }

        public IEnumerable<byte> ReadHeaders()
        {
            var stream = _tcpClient.GetStream();

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is unable to read");
            }

            var sequenceToFind = Encoding.UTF8.GetBytes($"{Environment.NewLine}{Environment.NewLine}");
            var result = new List<byte>();

            bool endOfHeadersReached = false;

            while (!endOfHeadersReached)
            {
                var currentValue = stream.ReadByte();

                result.Add((byte)currentValue);

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

            var stream = _tcpClient.GetStream();

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is unable to read");
            }

            var buffer = new byte[bodyLength];
            int bytesRead = 0;

            while (bytesRead < bodyLength)
            {
                bytesRead += await stream.ReadAsync(buffer, 0, buffer.Length);
            }

            return buffer;
        }

        public void CloseConnection() => _tcpClient?.Close();
    }
}
