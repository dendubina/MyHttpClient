﻿using System;
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
        private const HttpStatusCode ExpectedStatusCode = HttpStatusCode.OK;
        private const string ExampleUri = "http://google.com";
        private const string ExpectedResponseContent = "example content";

        private static readonly IHttpBody _exampleRequestContent = new StringBody("request content");

        private static readonly IEnumerable<byte> _expectedResponseContentBytes =
            Encoding.UTF8.GetBytes(ExpectedResponseContent);

        private static readonly IDictionary<string, string> _expectedHeaders = new Dictionary<string, string>
        {
            { "Connection", "keep-alive" },
            { "Server", "gws" },
            { "Content-Length", _expectedResponseContentBytes.Count().ToString() }
        };

        private static readonly string _responseHeadersString =
            $"HTTP/1.1 {(int)ExpectedStatusCode} OK{Environment.NewLine}" +
            $"Connection: keep-alive{Environment.NewLine}" +
            $"Server: gws{Environment.NewLine}" +
            $"Content-Length: {_expectedResponseContentBytes.Count()}{Environment.NewLine}";

        private readonly IMyHttpClient _httpClient;

        public MyHttpClientExtensionTests()
        {
            var mockedConnection = new Mock<IConnection>();

            mockedConnection
                .Setup(x => x.ReadHeaders())
                .Returns(Encoding.UTF8.GetBytes(_responseHeadersString));

            mockedConnection
                .Setup(x => x.ReadBodyAsync(It.IsAny<int>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(ExpectedResponseContent));

            _httpClient = new MyHttpClientProject.MyHttpClient(mockedConnection.Object);
        }

        [Fact]
        public async void GetAsync_Returns_Expected_HttpResponse()
        {
            //Act
            var actual = await _httpClient.GetAsync(ExampleUri);

            //Assert
            Assert.Equal(ExpectedStatusCode, actual.StatusCode);
            Assert.Equal(_expectedHeaders, actual.ResponseHeaders);
            Assert.Equal(Encoding.UTF8.GetBytes(ExpectedResponseContent), actual.ResponseBody.ToArray());
        }

        [Fact]
        public async void PostAsync_Returns_Expected_HttpResponse()
        {
            //Act
            var actual = await _httpClient.PostAsync(ExampleUri, _exampleRequestContent);

            //Assert
            Assert.Equal(ExpectedStatusCode, actual.StatusCode);
            Assert.Equal(_expectedHeaders, actual.ResponseHeaders);
            Assert.Equal(Encoding.UTF8.GetBytes(ExpectedResponseContent), actual.ResponseBody.ToArray());
        }

        [Fact]
        public async void PostWithStringResponseAsync_Returns_Expected_String()
        {
            //Act
            var actual = await _httpClient.PostWithStringResponseAsync(ExampleUri, _exampleRequestContent);

            //Assert
            Assert.Equal(Encoding.UTF8.GetString(_expectedResponseContentBytes.ToArray()), actual);
        }

        [Fact]
        public async void PostWithByteArrayResponseAsync_Returns_Expected_ByteArray()
        {
            //Act
            var actual = await _httpClient.PostWithByteArrayResponseAsync(ExampleUri, _exampleRequestContent);

            //Assert
            Assert.Equal(_expectedResponseContentBytes.ToArray(), actual);
        }

        [Fact]
        public async void PostWithStreamResponseAsync_Returns_Expected_Stream()
        {
            //Arrange
            using var memoryStream = new MemoryStream();

            //Act
            var actual = await _httpClient.PostWithStreamResponseAsync(ExampleUri, _exampleRequestContent);
            await actual.CopyToAsync(memoryStream);

            //Assert
            Assert.Equal(_expectedResponseContentBytes.ToArray(), memoryStream.ToArray());
        }
    }
}
