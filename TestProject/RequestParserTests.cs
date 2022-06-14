using System;
using System.Net.Http;
using System.Text;
using MyHttpClientProject.Builders;
using MyHttpClientProject.HttpBody;
using MyHttpClientProject.Parsers;
using Xunit;

namespace TestProject
{
    public class RequestParserTests
    {
        [Fact]
        public void ParseToHttpRequestBytes_ValidParameter_ReturnsExpectedByteArray()
        {
            //Arrange
            const string body = "<HTML>body</HTML>";
            string expected = "GET http://google.com/ HTTP/1.1" + Environment.NewLine +
                              "Content-Type: text/plain; charset=utf-8" + Environment.NewLine +
                              $"Content-Length: {Encoding.UTF8.GetByteCount(body)}" + Environment.NewLine +
                              "Host: google.com" + Environment.NewLine +
                              Environment.NewLine +
                              body;

            var builder = new RequestOptionsBuilder();

            var options = builder
                .SetMethod(HttpMethod.Get)
                .SetUri("http://google.com")
                .AddBody(new StringBody(body))
                .GetResult();

            //Act
            var actual = RequestParser.ParseToHttpRequestBytes(options);

            //Assert
            Assert.Equal(Encoding.UTF8.GetBytes(expected), actual);
        }
    }
}
