using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FluentAssertions;
using MyHttpClientProject.Services.Parsers;
using Xunit;

namespace MyHttpClientProject.Tests.Services.Parsers
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
            //Act 
            Action act = () => ResponseHeadersParser.ParseFromBytes(_encoding.GetBytes(statusLine));

            //Assert
            act.Should().Throw<FormatException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new byte[]{})]
        public void ParseFromBytes_Should_ThrowException_When_NullOrEmpty_Data(byte[] data)
        {
            //Act 
            Action act = () => ResponseHeadersParser.ParseFromBytes(data);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ParseFromBytes_Should_ThrowException_When_Response_Without_Headers()
        {
            //Arrange
            const string responseWithNoHeaders = "HTTP/1.1 200 OK";

            //Act
            Action act = () => ResponseHeadersParser.ParseFromBytes(_encoding.GetBytes(responseWithNoHeaders));

            //Assert
            act.Should().Throw<FormatException>();
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
            string responseWithInvalidHeader = $"HTTP/1.1 200 OK{Environment.NewLine}" +
                                               $"Server: Apache{Environment.NewLine}" +
                                               invalidHeader;

            //Act
            Action act = () => ResponseHeadersParser.ParseFromBytes(_encoding.GetBytes(responseWithInvalidHeader));

            //Assert
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void ParseFromBytes_Should_Return_Expected_HttpResponse_Instance_When_Valid_Response_Bytes()
        {
            //Arrange
            var expectedHeaders = new Dictionary<string, string>
            {
                { "Location", "http://www.google.com/" },
                { "Connection", "close" },
            };

            string response = $"HTTP/1.1 301 Moved Permanently{Environment.NewLine}" + 
                              $"Location: http://www.google.com/{Environment.NewLine}" +
                              $"Connection: close{Environment.NewLine}" +
                              $"{Environment.NewLine}";

            //Act
            var actual = ResponseHeadersParser.ParseFromBytes(_encoding.GetBytes(response));

            //Assert
            actual.StatusCode.Should().Be(HttpStatusCode.Moved);
            actual.Headers.Should().Equal(expectedHeaders);
        }
    }
}
