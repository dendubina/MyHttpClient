using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FluentAssertions;
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
        public void Build_Should_ThrowException_When_Uri_NotSet()
        {
            //Arrange
            _builder.SetMethod(HttpMethod.Get);

            //Act
            Action act = () => _builder.Build();

            //Assert
            act.Should().Throw<OptionsBuildingException>();
        }

        [Fact]
        public void Build_Should_ThrowException_When_MethodNotSet()
        {
            //Arrange
            _builder.SetUri("http://google.com");

            //Act
            Action act = () => _builder.Build();

            //Assert
            act.Should().Throw<OptionsBuildingException>();
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
            //Act
            Action act = () => _builder.AddHeader(name, value);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AddHeader_Should_ThrowException_When_Adding_Header_With_Same_Name_Twice()
        {
            //Arrange
            _builder.AddHeader("name", "value");

            //Act
            Action act = () => _builder.AddHeader("name", "value");

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AddHeader_Should_AddHeader_When_ValidParameters()
        {
            //Act
            var result = _builderWithRequiredValues
                .AddHeader("name", "value")
                .Build();

            //Assert
            result.Headers.Should().Contain(new KeyValuePair<string, string>("name", "value"));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void SetReceiveTimeout_Should_ThrowException_When_Invalid_Parameter(int milliseconds)
        {
            //Act
            Action act = () => _builder.SetReceiveTimeout(milliseconds);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void SetSendTimeout_Should_ThrowException_When_Invalid_Parameter(int milliseconds)
        {
            //Act
            Action act = () => _builder.SetSendTimeout(milliseconds);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void SetReceiveTimeoutShould_Set_Value_To_RequestOptions_Property_When_Valid_Parameter(int milliseconds)
        {
            //Act 
            var options = _builderWithRequiredValues
                .SetReceiveTimeout(milliseconds)
                .Build();

            //Assert
            options.ReceiveTimeout.Should().Be(milliseconds);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void SetSendTimeout_Should_Set_Value_To_RequestOptions_Property_When_Valid_Parameter(int milliseconds)
        {
            //Act 
            var options = _builderWithRequiredValues
                .SetSendTimeout(milliseconds)
                .Build();

            //Assert
            options.SendTimeout.Should().Be(milliseconds);
        }

        [Fact]
        public void Build_Should_Add_Host_Header()
        {
            //Act
            var result = _builderWithRequiredValues.Build();

            //Assert
            result.Headers["Host"].Should().Be(result.Uri.Host);
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
            result.Headers["Content-Length"].Should().Be(Encoding.UTF8.GetByteCount(content).ToString());
            result.Headers["Content-Type"].Should().Be($"{contentType}; charset=utf-8");
        }
    }
}
