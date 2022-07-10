using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Moq;
using MyHttpClientProject.Models;
using MyHttpClientProject.Services.Interfaces;
using Xunit;

namespace TestProject.MyHttpClient
{
    public class MyHttpClientTests
    {
        private readonly Mock<IConnection> _mockedConnection;
        private readonly RequestOptions _requestOptions;

        public MyHttpClientTests()
        {
            _mockedConnection = new Mock<IConnection>();

            _requestOptions = new RequestOptions()
            {
                Uri = new Uri("http://google.com"),
                Method = HttpMethod.Get,
            };
        }

        [Fact]
        public async void GetResponseAsync_Should_Close_Connection_When_ConnectionClose_Header_Exists()
        {
            //Arrange
            var responseWithConnectionCloseHeader =
                "HTTP/1.1 200 OK" + Environment.NewLine +
                "Connection: close";

            _mockedConnection
                .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<IEnumerable<byte>>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(responseWithConnectionCloseHeader));

            var client = new MyHttpClientProject.MyHttpClient(_mockedConnection.Object);

            //Act
            await client.GetResponseAsync(_requestOptions);

            //Assert
            _mockedConnection.Verify(x => x.Dispose(), Times.Once);
        }

        [Theory]
        [InlineData("keep-alive")]
        [InlineData("invalid")]
        public async void GetResponseAsync_Should_Not_Close_Connection_When_Connection_Header_Has_KeepAlive_Or_Invalid_Value(string value)
        {
            //Arrange
            var response =
                "HTTP/1.1 200 OK" + Environment.NewLine +
                $"Connection: {value}";

            _mockedConnection
                .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<IEnumerable<byte>>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(response));

            var client = new MyHttpClientProject.MyHttpClient(_mockedConnection.Object);

            //Act
            await client.GetResponseAsync(_requestOptions);

            //Assert
            _mockedConnection.Verify(x => x.Dispose(), Times.Never);
        }
    }
}
