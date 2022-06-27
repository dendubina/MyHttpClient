using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MyHttpClientProject.Builders;
using MyHttpClientProject.Exceptions;
using MyHttpClientProject.HttpBody;
using Xunit;

namespace TestProject.Builders
{
    public class RequestOptionsBuilderTests
    {
        private readonly IRequestOptionsBuilder _builder;

        public RequestOptionsBuilderTests()
        {
            _builder = new RequestOptionsBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri("http://google.com");
        }

        [Theory]
        [InlineData("")]
        [InlineData("http:///google.com")]
        [InlineData("google.com")]
        public void SetUri_Should_ThrowUriFormatException_When_InvalidValue(string value)
        {
            //Arrange
            var builder = new RequestOptionsBuilder();

            //Act and Assert
            Assert.Throws<UriFormatException>(() => builder.SetUri(value));
        }

        [Fact]
        public void Build_Should_ThrowsException_When_Uri_NotSet()
        {
            //Arrange
            var builder = new RequestOptionsBuilder();
            builder.SetMethod(HttpMethod.Get);

            //Act and Assert
            Assert.Throws<OptionsBuildingException>(() => builder.Build());
        }

        [Fact]
        public void Build_Should_ThrowException_When_MethodNotSet()
        {
            //Arrange
            var builder = new RequestOptionsBuilder();
            builder.SetUri("http://google.com");

            //Act and Assert
            Assert.Throws<OptionsBuildingException>(() => builder.Build());
        }

        [Theory]
        [InlineData("", "value")]
        [InlineData("name", " ")]
        [InlineData("na\rme", "value")]
        [InlineData("name", "val\nue")]
        [InlineData("name", null)]
        [InlineData(null, "value")]
        public void AddHeader_Should_ThrowException_When_InvalidParameter(string name, string value)
        {
            //Arrange
            var builder = new RequestOptionsBuilder();

            //Act and Assert
            Assert.Throws<ArgumentException>(()=> builder.AddHeader(name, value));
        }

        [Fact]
        public void AddHeader_Should_AddHeader_When_ValidParameters()
        {
            //Act
            var result = _builder
                .AddHeader("name", "value")
                .Build();

            //Assert
            Assert.Contains(new KeyValuePair<string,string>("name", "value"), result.Headers);
        }

        [Fact]
        public void Build_Should_Add_Host_Header()
        {
            //Act
            var result = _builder.Build();

            //Assert
            Assert.Contains("Host", result.Headers);
        }

        [Fact]
        public void Build_Should_Add_ContentTypeHeader_When_Body_Has_MediaType()
        {
            //Act
            var result = _builder
                .SetBody(new StringBody("content", Encoding.UTF8, "application/json"))
                .Build();

            //Assert
            Assert.Contains("Content-Type", result.Headers);
        }

        [Fact]
        public void Build_Should_Add_ContentLengthHeader_When_Body_Added()
        {
            //Act
            var result = _builder
                .SetBody(new StringBody("content", Encoding.UTF8, "application/json"))
                .Build();

            //Assert
            Assert.Contains("Content-Length", result.Headers);
        }
    }
}
