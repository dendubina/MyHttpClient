using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MyHttpClientProject.Parsers;
using Xunit;

namespace TestProject
{
    public class ResponseParserTests
    {
        private readonly Encoding _encoding = Encoding.UTF8;

        [Theory]
        [InlineData("Invalid status line")]
        [InlineData(" ")]
        public void ParseFromBytes_InvalidStatusLine_ThrowsException(string statusLine)
        {
            //Act and Assert
            Assert.Throws<FormatException>(() => ResponseParser.ParseFromBytes(_encoding.GetBytes(statusLine)));
        }

        [Fact]
        public void ParseFromBytes_NoHeadersFound_ThrowsException()
        {
            //Arrange
            const string responseWithNoHeaders = "HTTP/1.1 200 OK";

            //Act and Assert
            Assert.Throws<FormatException>(() => ResponseParser.ParseFromBytes(_encoding.GetBytes(responseWithNoHeaders)));
        }

        [Theory]
        [InlineData("invalid header")]
        [InlineData(":invalid header")]
        [InlineData("invalid ed:e")]
        [InlineData("invalid header:")]
        public void ParseFromBytes_InvalidHeaderFound_ThrowsException(string invalidHeader)
        {
            //Arrange
            string responseWithInvalidHeader = "HTTP/1.1 200 OK" + Environment.NewLine +
                                               "Server: Apache" + Environment.NewLine +
                                               invalidHeader;

            //Act and Assert
            Assert.Throws<FormatException>(() => ResponseParser.ParseFromBytes(_encoding.GetBytes(responseWithInvalidHeader)));
        }

        [Fact]
        public void ParseFromBytes_ValidResponseBytes_ReturnsExpectedHttpResponseInstance()
        {
            //Arrange
            const string body = "example body";
            int bodyByteCount = _encoding.GetByteCount(body);
            var headers = new Dictionary<string, string>
            {
                { "Location", "http://www.google.com/" },
                { "Connection", "close" },
                { "Content-Length", $"{bodyByteCount}" },
            };
            string responseWithBody = "HTTP/1.1 301 Moved Permanently" + Environment.NewLine +
                                      "Location: http://www.google.com/" + Environment.NewLine +
                                      "Connection: close" + Environment.NewLine +
                                      $"Content-Length: {bodyByteCount}" + Environment.NewLine +
                                      Environment.NewLine +
                                      body;

            //Act
            var result = ResponseParser.ParseFromBytes(_encoding.GetBytes(responseWithBody));

            //Assert
            Assert.Equal(HttpStatusCode.Moved, result.StatusCode);
            Assert.Equal(headers, result.ResponseHeaders);
            Assert.Equal(body, _encoding.GetString(result.ResponseBody.ToArray()));
        }
    }
}
