using System;
using System.IO;

namespace MyHttpClientProject.Services.Interfaces
{
    public interface IClient
    {
        Stream GetStream();
        bool IsConnected(string address, ushort port);
        void OpenConnection(string address, ushort port, int sendTimeout, int receiveTimeout);
        void CloseConnection();
    }
}
