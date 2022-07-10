using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHttpClientProject.Services.Interfaces
{
    public interface IConnection
    {
        Task SendRequestAsync(string address, ushort port, IEnumerable<byte> data);
        IEnumerable<byte> ReadHeaders();
        Task<IEnumerable<byte>> ReadBody(int bodyLength);
        void CloseConnection();
    }
}
