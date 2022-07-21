using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FluentAssertions;
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
        private const HttpStatusCode ExpectedSuccessStatusCode = HttpStatusCode.OK;
        private const string SomeTestUri = "http://google.com";

        private static readonly IHttpBody _someRequestContent = new StringBody("request content");

        private static readonly IEnumerable<byte> _expectedResponseContentBytes =
            Encoding.UTF8.GetBytes("example content");

        private static readonly IDictionary<string, string> _expectedHeaders = new Dictionary<string, string>
        {
            { "Connection", "keep-alive" },
            { "Server", "gws" },
            { "Content-Length", _expectedResponseContentBytes.Count().ToString() }
        };

        private static readonly string _responseHeadersString =
            $"HTTP/1.1 {(int)ExpectedSuccessStatusCode} OK{Environment.NewLine}" +
            $"Connection: keep-alive{Environment.NewLine}" +
            $"Server: gws{Environment.NewLine}" +
            $"Content-Length: {_expectedResponseContentBytes.Count()}{Environment.NewLine}";

        private readonly IMyHttpClient _httpClient;

        public MyHttpClientExtensionTests()
        {
            var mockedConnection = new Mock<IConnectionHandler>();

            mockedConnection
                .Setup(x => x.ReadHeaders())
                .Returns(Encoding.UTF8.GetBytes(_responseHeadersString));

            mockedConnection
                .Setup(x => x.ReadBodyAsync(It.IsAny<int>()))
                .ReturnsAsync(_expectedResponseContentBytes);

            _httpClient = new MyHttpClientProject.MyHttpClient(mockedConnection.Object);
        }

        [Fact]
        public async void GetAsync_Returns_Expected_HttpResponse()
        {
            //Act
            var actualResponse = await _httpClient.GetAsync(SomeTestUri);

            //Assert
            actualResponse.StatusCode.Should().Be(ExpectedSuccessStatusCode);
            actualResponse.ResponseHeaders.Should().Equal(_expectedHeaders);
            actualResponse.ResponseBody.Should().Equal(_expectedResponseContentBytes);
        }

        [Fact]
        public async void PostAsync_Returns_Expected_HttpResponse()
        {
            //Act
            var actualResponse = await _httpClient.PostAsync(SomeTestUri, _someRequestContent);

            //Assert
            actualResponse.StatusCode.Should().Be(ExpectedSuccessStatusCode);
            actualResponse.ResponseHeaders.Should().Equal(_expectedHeaders);
            actualResponse.ResponseBody.Should().Equal(_expectedResponseContentBytes);
        }

        [Fact]
        public async void PostWithStringResponseAsync_Returns_Expected_String()
        {
            //Act
            var actualResponse = await _httpClient.PostWithStringResponseAsync(SomeTestUri, _someRequestContent);

            //Assert
            actualResponse.Should().Be(Encoding.UTF8.GetString(_expectedResponseContentBytes.ToArray()));
        }

        [Fact]
        public async void PostWithByteArrayResponseAsync_Returns_Expected_ByteArray()
        {
            //Act
            var actualResponse = await _httpClient.PostWithByteArrayResponseAsync(SomeTestUri, _someRequestContent);

            //Assert
            actualResponse.Should().Equal(_expectedResponseContentBytes.ToArray());
        }

        [Fact]
        public async void PostWithStreamResponseAsync_Returns_Expected_Stream()
        {
            //Arrange
            using var memoryStream = new MemoryStream();

            //Act
            var actualResponse = await _httpClient.PostWithStreamResponseAsync(SomeTestUri, _someRequestContent);
            await actualResponse.CopyToAsync(memoryStream);

            //Assert
            memoryStream.ToArray().Should().Equal(_expectedResponseContentBytes.ToArray());
        }
    }
}
