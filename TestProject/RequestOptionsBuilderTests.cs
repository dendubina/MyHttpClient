using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MyHttpClientProject.Builders;
using MyHttpClientProject.Exceptions;
using MyHttpClientProject.HttpBody;
using Xunit;

namespace TestProject
{
    public class RequestOptionsBuilderTests
    {
        [Fact]
        public void GetResult_UriNotSet_ThrowsException()
        {
            //Arrange
            var builder = new RequestOptionsBuilder();
            builder.SetMethod(HttpMethod.Get);

            //Act and Assert
            Assert.Throws<OptionsBuildingException>(() => builder.GetResult());
        }

        [Fact]
        public void GetResult_MethodNotSet_ThrowsException()
        {
            //Arrange
            var builder = new RequestOptionsBuilder();
            builder.SetUri("http://google.com");

            //Act and Assert
            Assert.Throws<OptionsBuildingException>(() => builder.GetResult());
        }

        [Theory]
        [InlineData(" ", "value")]
        [InlineData("name", " ")]
        [InlineData("na\rme", "value")]
        [InlineData("name", "val\nue")]
        public void AddHeader_InvalidParameter_ThrowsException(string name, string value)
        {
            //Arrange
            var builder = new RequestOptionsBuilder();

            //Act and Assert
            Assert.Throws<ArgumentException>(()=> builder.AddHeader(name, value));
        }

        [Fact]
        public void AddHeader_ValidParameters_AddsHeader()
        {
            //Arrange
            const string name = "name";
            const string value = "value";
            var expected = new Dictionary<string, string> { {"Host", "google.com" }, { name, value } };
            var builder = new RequestOptionsBuilder();
            builder
                .SetMethod(HttpMethod.Get)
                .SetUri("http://google.com");

            //Act
            var result = builder
                .AddHeader(name, value)
                .GetResult();

            //Assert
            Assert.Equal(expected, result.Headers);
        }

        [Fact]
        public void AddBody_MediaTypeNotBull_AddsContentTypeHeader()
        {
            //Arrange
            var body = new StringBody("content", Encoding.UTF8, "application/json");
            var builder = new RequestOptionsBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri("http://google.com");

            //Act
            var result = builder
                .AddBody(body)
                .GetResult();

            //Assert
            Assert.Contains("Content-Type", result.Headers);
        }

        [Fact]
        public void AddBody_HasContent_AddsContentLenghtHeader()
        {
            //Arrange
            var body = new StringBody("content", Encoding.UTF8, "application/json");
            var builder = new RequestOptionsBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri("http://google.com");

            //Act
            var result = builder
                .AddBody(body)
                .GetResult();

            //Assert
            Assert.Contains("Content-Length", result.Headers);
        }
    }
}
