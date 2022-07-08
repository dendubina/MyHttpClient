using System;
using System.IO;

namespace MyHttpClientProject.Services.Interfaces
{
    public interface ITcpClient : IDisposable
    {
        void OpenNewConnection(string address, ushort port);
        bool Connected(string address, ushort port);
        Stream GetStream();
        void Close();
    }
}
