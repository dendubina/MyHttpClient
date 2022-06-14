using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHttpClientProject.WebConnection
{
    public interface IWebConnection : IDisposable
    {
        Task<byte[]> Send(string address, ushort port, IEnumerable<byte> data);
    }
}
