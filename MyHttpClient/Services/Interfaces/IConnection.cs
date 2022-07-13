using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHttpClientProject.Services.Interfaces
{
    public interface IConnection : IDisposable
    {
        int SendTimeout { get; set; }
        int ReadTimeout { get; set; }

        Task SendRequestAsync(string address, ushort port, IEnumerable<byte> data);
        IEnumerable<byte> ReadHeaders();
        Task<IEnumerable<byte>> ReadBody(int bodyLength);
        void CloseConnection();
    }
}
