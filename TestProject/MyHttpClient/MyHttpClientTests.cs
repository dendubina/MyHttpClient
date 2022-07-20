using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Moq;
using MyHttpClientProject;
using MyHttpClientProject.Models;
using MyHttpClientProject.Services.Interfaces;
using Xunit;

namespace TestProject.MyHttpClient
{
    public class MyHttpClientTests
    {
        private readonly Mock<IConnectionHandler> _mockedConnection;
        private readonly RequestOptions _requestOptions;
        private readonly IMyHttpClient _httpClient;

        public MyHttpClientTests()
        {
            _mockedConnection = new Mock<IConnectionHandler>();

            _httpClient = new MyHttpClientProject.MyHttpClient(_mockedConnection.Object);

            _requestOptions = new RequestOptions
            {
                Uri = new Uri("http://google.com"),
                Method = HttpMethod.Get,

                Headers = new Dictionary<string, string>
                {
                    {"Host", "google.com"},
                }
            };
        }

        [Fact]
        public void GetResponseAsync_Should_ThrowException_When_Parameter_Null()
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _httpClient.GetResponseAsync(null));
        }

        [Theory]
        [InlineData("close")]
        [InlineData("Close")]
        [InlineData("CLOSE")]
        public async void GetResponseAsync_Should_Close_Connection_When_ConnectionClose_Header_Exists(string headerValue)
        {
            //Arrange
            var responseWithConnectionCloseHeader =
                $"HTTP/1.1 200 OK{Environment.NewLine}" +
                $"Connection: {headerValue}";

            _mockedConnection
                .Setup(x => x.ReadHeaders())
                .Returns(Encoding.UTF8.GetBytes(responseWithConnectionCloseHeader));

            //Act
            await _httpClient.GetResponseAsync(_requestOptions);

            //Assert
            _mockedConnection.Verify(x => x.CloseConnection(), Times.Once);
        }

        [Theory]
        [InlineData("keep-alive")]
        [InlineData("KEEP-ALIVE")]
        [InlineData("Keep-Alive")]
        [InlineData("invalid")]
        public async void GetResponseAsync_Should_Not_Close_Connection_When_Connection_Header_Has_KeepAlive_Or_Invalid_Value(string headerValue)
        {
            //Arrange
            var response =
                $"HTTP/1.1 200 OK{Environment.NewLine}" +
                $"Connection: {headerValue}";

            _mockedConnection
                .Setup(x => x.ReadHeaders())
                .Returns(Encoding.UTF8.GetBytes(response));

            //Act
            await _httpClient.GetResponseAsync(_requestOptions);

            //Assert
            _mockedConnection.Verify(x => x.CloseConnection(), Times.Never);
        }

        [Fact]
        public async void GetResponseAsync_Should_Not_Try_Read_Body_When_Response_Has_No_ContentLength_Header()
        {
            //Arrange
            var response =
                $"HTTP/1.1 200 OK{Environment.NewLine}" +
                $"Connection: keep-alive{Environment.NewLine}" +
                $"Server: gws";

            _mockedConnection
                .Setup(x => x.ReadHeaders())
                .Returns(Encoding.UTF8.GetBytes(response));

            //Act
            await _httpClient.GetResponseAsync(_requestOptions);

            //Assert
            _mockedConnection.Verify(x => x.ReadBodyAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async void GetResponseAsync_Should_Return_Expected_Response_When_Valid_Response()
        {
            //Arrange
            var expectedStatusCode = HttpStatusCode.OK;

            var content = "example content";
            int contentLength = Encoding.UTF8.GetByteCount(content);

            var expectedHeaders = new Dictionary<string, string>
            {
                { "Connection", "keep-alive" },
                { "Server", "gws" },
                { "Content-Length", contentLength.ToString() }
            };

            var responseHeadersString =
                $"HTTP/1.1 {(int)expectedStatusCode} OK{Environment.NewLine}" +
                $"Connection: keep-alive{Environment.NewLine}" +
                $"Server: gws{Environment.NewLine}" +
                $"Content-Length: {contentLength}{Environment.NewLine}";

            _mockedConnection
                .Setup(x => x.ReadHeaders())
                .Returns(Encoding.UTF8.GetBytes(responseHeadersString));
            
            _mockedConnection
                .Setup(x => x.ReadBodyAsync(contentLength))
                .ReturnsAsync(Encoding.UTF8.GetBytes(content));

            //Act
            var actual = await _httpClient.GetResponseAsync(_requestOptions);

            //Assert
            Assert.Equal(expectedStatusCode, actual.StatusCode);
            Assert.Equal(expectedHeaders, actual.ResponseHeaders);
            Assert.Equal(content, Encoding.UTF8.GetString(actual.ResponseBody.ToArray()));
        }
    }
}
