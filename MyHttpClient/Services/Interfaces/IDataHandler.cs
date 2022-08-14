using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHttpClientProject.Services.Interfaces
{
    public interface IDataHandler
    {
        int SendTimeout { get; set; }
        int ReceiveTimeout { get; set; }
        int ReceiveBufferSize { get; set; }

        Task SendAsync(string address, ushort port, IEnumerable<byte> data);
        IEnumerable<byte> ReadHeaders();
        Task<IEnumerable<byte>> ReadBodyAsync(int bodyLength);
        void CloseConnection();
    }
}
