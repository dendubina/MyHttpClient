using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MyHttpClientProject.Builders;
using MyHttpClientProject.Exceptions;
using MyHttpClientProject.HttpBody;
using MyHttpClientProject.Models;
using Xunit;

namespace TestProject.Builders
{
    public class RequestOptionsBuilderTests
    {
        private readonly RequestOptions _optionsWithBodyAndCustomHeader;

        public RequestOptionsBuilderTests()
        {
            _optionsWithBodyAndCustomHeader = new RequestOptionsBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri("http://google.com")
                .AddHeader("name", "value")
                .SetBody(new StringBody("content", Encoding.UTF8, "application/json"))
                .Build();
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
            //Assert
            Assert.Contains(new KeyValuePair<string,string>("name", "value"), _optionsWithBodyAndCustomHeader.Headers);
        }

        [Fact]
        public void Build_Should_Add_Host_Header()
        {
            //Assert
            Assert.Contains("Host", _optionsWithBodyAndCustomHeader.Headers);
        }

        [Fact]
        public void SetBody_Should_Set_Body_Property()
        {
            //Assert
            Assert.NotNull(_optionsWithBodyAndCustomHeader.Body);
        }

        [Fact]
        public void Build_Should_Add_ContentTypeHeader_When_Body_Has_MediaType()
        {
            //Assert
            Assert.Contains("Content-Type", _optionsWithBodyAndCustomHeader.Headers);
        }

        [Fact]
        public void Build_Should_Add_ContentLengthHeader_When_Body_Added()
        {
            //Assert
            Assert.Contains("Content-Length", _optionsWithBodyAndCustomHeader.Headers);
        }
    }
}
