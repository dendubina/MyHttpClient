using System;
using System.IO;

namespace MyHttpClientProject.Services.Interfaces
{
    public interface IClient
    {
        Stream GetStream();
        bool Connected(string address, ushort port);
        void OpenNewConnection(string address, ushort port, int sendTimeout, int receiveTimeout);
        void CloseConnection();
    }
}
