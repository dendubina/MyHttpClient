using System;
using System.IO;

namespace MyHttpClientProject.Services.Interfaces
{
    public interface IConnection
    {
        Stream GetStream();
        bool IsConnected(string address, ushort port);
        void OpenConnection(string address, ushort port, int sendTimeout, int receiveTimeout);
        void CloseConnection();
    }
}
