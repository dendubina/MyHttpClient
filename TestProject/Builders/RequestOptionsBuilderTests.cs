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
        private readonly IRequestOptionsBuilder _builderWithRequiredValues;
        private readonly IRequestOptionsBuilder _builder;

        public RequestOptionsBuilderTests()
        {
            _builder = new RequestOptionsBuilder();

            _builderWithRequiredValues = new RequestOptionsBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri("http://google.com");
        }

        [Fact]
        public void Build_Should_ThrowsException_When_Uri_NotSet()
        {
            //Arrange
            _builder.SetMethod(HttpMethod.Get);

            //Act and Assert
            Assert.Throws<OptionsBuildingException>(() => _builder.Build());
        }

        [Fact]
        public void Build_Should_ThrowException_When_MethodNotSet()
        {
            //Arrange
            _builder.SetUri("http://google.com");

            //Act and Assert
            Assert.Throws<OptionsBuildingException>(() => _builder.Build());
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
            //Act and Assert
            Assert.Throws<ArgumentException>(()=> _builder.AddHeader(name, value));
        }

        [Fact]
        public void AddHeader_Should_ThrowException_When_Adding_Header_With_Same_Name_Twice()
        {
            //Arrange
            _builder.AddHeader("name", "value");

            //Act and Assert
            Assert.Throws<ArgumentException>(() => _builder.AddHeader("name", "value"));
        }

        [Fact]
        public void AddHeader_Should_AddHeader_When_ValidParameters()
        {
            //Act
            var result = _builderWithRequiredValues
                .AddHeader("name", "value")
                .Build();

            //Assert
            Assert.Contains(new KeyValuePair<string,string>("name", "value"), result.Headers);
        }

        [Fact]
        public void Build_Should_Add_Host_Header()
        {
            //Act
            var result = _builderWithRequiredValues.Build();

            //Assert
            Assert.Equal(result.Uri.Host, result.Headers["Host"]);
        }

        [Fact]
        public void Build_Should_Add_Expected_RepresentationHeaders_When_Body_Added()
        {
            //Arrange
            const string content = "content";
            const string contentType = "application/json";

            //Act
            var result = _builderWithRequiredValues
                .SetBody(new StringBody(content, Encoding.UTF8, contentType))
                .Build();

            //Assert
            Assert.Contains(result.Headers["Content-Length"], Encoding.UTF8.GetByteCount(content).ToString());
            Assert.Contains(result.Headers["Content-Type"], $"{contentType}; charset=utf-8");
        }
    }
}
