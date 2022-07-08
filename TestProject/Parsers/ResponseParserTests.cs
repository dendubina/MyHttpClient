using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MyHttpClientProject.Parsers;
using Xunit;

namespace TestProject.Parsers
{
    public class ResponseParserTests
    {
        private readonly Encoding _encoding = Encoding.UTF8;

        [Theory]
        [InlineData("Invalid status line")]
        [InlineData("Status")]
        [InlineData("Status line")]
        [InlineData(" ")]
        public void ParseFromBytes_Should_ThrowException_When_Invalid_Status_Line(string statusLine)
        {
            //Act and Assert
            Assert.Throws<FormatException>(() => ResponseParser.ParseFromBytes(_encoding.GetBytes(statusLine)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new byte[]{})]
        public void ParseFromBytes_Should_ThrowException_When_NullOrEmpty_Data(byte[] data)
        {
            //Act and Assert
            Assert.Throws<ArgumentException>(() => ResponseParser.ParseFromBytes(data));
        }

        [Fact]
        public void ParseFromBytes_Should_ThrowException_When_No_Headers_Found_()
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
        [InlineData(" :  ")]
        [InlineData("header")]
        [InlineData(":header")]
        [InlineData("header:")]
        public void ParseFromBytes_Should_ThrowException_When_InvalidHeaderFound(string invalidHeader)
        {
            //Arrange
            string responseWithInvalidHeader = "HTTP/1.1 200 OK" + Environment.NewLine +
                                               "Server: Apache" + Environment.NewLine +
                                               invalidHeader;

            //Act and Assert
            Assert.Throws<FormatException>(() => ResponseParser.ParseFromBytes(_encoding.GetBytes(responseWithInvalidHeader)));
        }

        [Fact]
        public void ParseFromBytes_Should_Return_Expected_HttpResponse_Instance_When_ValidResponseBytes()
        {
            //Arrange
            var headers = new Dictionary<string, string>
            {
                { "Location", "http://www.google.com/" },
                { "Connection", "close" },
            };
            string responseWithBody = "HTTP/1.1 301 Moved Permanently" + Environment.NewLine +
                                      "Location: http://www.google.com/" + Environment.NewLine +
                                      "Connection: close" + Environment.NewLine +
                                      Environment.NewLine;
                                     

            //Act
            var result = ResponseParser.ParseFromBytes(_encoding.GetBytes(responseWithBody));

            //Assert
            Assert.Equal(HttpStatusCode.Moved, result.StatusCode);
            Assert.Equal(headers, result.ResponseHeaders);
        }
    }
}
