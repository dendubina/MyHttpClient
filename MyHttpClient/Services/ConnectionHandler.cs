using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MyHttpClientProject.Services.Interfaces;

namespace MyHttpClientProject.Services
{
    public class ConnectionHandler : IConnectionHandler
    {
        public int ReceiveTimeout { get; set; }
        public int SendTimeout { get; set; }

        public int ReceiveBufferSize
        {
            get => receiveBufferSize; 
            set => receiveBufferSize = value > 0 ? value : throw new ArgumentException("Value must be > 0");
        }

        private int receiveBufferSize = 8192;
        private readonly IClient _client;

        public ConnectionHandler() : this(new MyTcpClient())
        {
            
        }

        public ConnectionHandler(IClient client)
        {
            _client = client;
        }

        public async Task SendAsync(string address, ushort port, IEnumerable<byte> data)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address must not be null or empty", nameof(address));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data must not be null");
            }

            if (!_client.Connected(address, port))
            {
                _client.OpenNewConnection(address, port, SendTimeout, ReceiveTimeout);
            }

            var dataArray = data.ToArray();

            await _client.GetStream().WriteAsync(dataArray, 0, dataArray.Length);
        }

        public IEnumerable<byte> ReadHeaders()
        {
            var stream = _client.GetStream();

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is unable to read");
            }

            var sequenceToFind = Encoding.UTF8.GetBytes($"{Environment.NewLine}{Environment.NewLine}");
            var result = new List<byte>();

            bool endOfHeadersReached = false;

            while (!endOfHeadersReached)
            {
                var current = stream.ReadByte();

                if (current == -1 || stream is NetworkStream { DataAvailable: false })
                {
                    throw new InvalidOperationException("Invalid response");
                }

                result.Add((byte)current);

                endOfHeadersReached = result
                    .TakeLast(sequenceToFind.Length)
                    .SequenceEqual(sequenceToFind);
            }

            if (result.Count == sequenceToFind.Length)
            {
                throw new InvalidOperationException("Response has no data");
            }
            
            return result;
        }

        public async Task<IEnumerable<byte>> ReadBodyAsync(int bodyLength)
        {
            if (bodyLength <= 0)
            {
                throw new ArgumentException("Body length must be > 0");
            }

            var stream = _client.GetStream();

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is unable to read");
            }

            var buffer = new byte[ReceiveBufferSize];

            using var response = new MemoryStream();

            do
            { 
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    throw new InvalidOperationException("Invalid response body length or response has no data");
                }

                await response.WriteAsync(buffer, 0, bytesRead);

            } while (response.Length != bodyLength);

            return response.ToArray();
        }

        public void CloseConnection() => _client.CloseConnection();
    }
}

