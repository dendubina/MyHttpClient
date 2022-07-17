using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Moq;
using MyHttpClientProject;
using MyHttpClientProject.Extensions;
using MyHttpClientProject.HttpBody;
using MyHttpClientProject.Services.Interfaces;
using Xunit;

namespace TestProject.Extensions
{
    public class MyHttpClientExtensionTests
    {
        private readonly IMyHttpClient _httpClient;
        private readonly Mock<IConnection> _mockedConnection;
        private readonly IDictionary<string, string> _expectedHeaders;
        private readonly HttpStatusCode _expectedStatusCode;
        private readonly IEnumerable<byte> _expectedContent;

        public MyHttpClientExtensionTests()
        {
            _expectedStatusCode = HttpStatusCode.OK;

            var content = "example content";
            int contentLength = Encoding.UTF8.GetByteCount(content);

            _expectedContent = Encoding.UTF8.GetBytes(content);

            _expectedHeaders = new Dictionary<string, string>
            {
                { "Connection", "keep-alive" },
                { "Server", "gws" },
                { "Content-Length", contentLength.ToString() }
            };

            var responseHeadersString =
                $"HTTP/1.1 {(int)_expectedStatusCode} OK{Environment.NewLine}" +
                $"Connection: keep-alive{Environment.NewLine}" +
                $"Server: gws{Environment.NewLine}" +
                $"Content-Length: {contentLength}{Environment.NewLine}";

            _mockedConnection = new Mock<IConnection>();

            _mockedConnection
                .Setup(x => x.ReadHeaders())
                .Returns(Encoding.UTF8.GetBytes(responseHeadersString));

            _mockedConnection
                .Setup(x => x.ReadBodyAsync(It.IsAny<int>()))
                .ReturnsAsync(_expectedContent);

            _httpClient = new MyHttpClientProject.MyHttpClient(_mockedConnection.Object);
        }

        [Fact]
        public async void GetAsync_Returns_Expected_HttpResponse()
        {
            //Act
            var actual = await _httpClient.GetAsync("http://google.com");

            //Assert
            Assert.Equal(_expectedStatusCode, actual.StatusCode);
            Assert.Equal(_expectedHeaders, actual.ResponseHeaders);
            Assert.Equal(_expectedContent, actual.ResponseBody.ToArray());
        }

        [Fact]
        public async void PostAsync_Returns_Expected_HttpResponse()
        {
            //Act
            var actual = await _httpClient.PostAsync("http://google.com", new StringBody("request content"));

            //Assert
            Assert.Equal(_expectedStatusCode, actual.StatusCode);
            Assert.Equal(_expectedHeaders, actual.ResponseHeaders);
            Assert.Equal(_expectedContent, actual.ResponseBody.ToArray());
        }

        [Fact]
        public async void PostWithStringResponseAsync_Returns_Expected_String()
        {
            //Act
            var actual = await _httpClient.PostWithStringResponseAsync("http://google.com", new StringBody("request content"));

            //Assert
            Assert.Equal(Encoding.UTF8.GetString(_expectedContent.ToArray()), actual);
        }

        [Fact]
        public async void PostWithByteArrayResponseAsync_Returns_Expected_ByteArray()
        {
            //Act
            var actual = await _httpClient.PostWithByteArrayResponseAsync("http://google.com", new StringBody("request content"));

            //Assert
            Assert.Equal(_expectedContent.ToArray(), actual);
        }

        [Fact]
        public async void PostWithStreamResponseAsync_Returns_Expected_Stream()
        {
            //Arrange
            using var memoryStream = new MemoryStream();

            //Act
            var actual = await _httpClient.PostWithStreamResponseAsync("http://google.com", new StringBody("request content"));
            await actual.CopyToAsync(memoryStream);

            //Assert
            Assert.Equal(_expectedContent.ToArray(), memoryStream.ToArray());
        }
    }
}
