using System;
using System.Collections.Generic;
using System.Net.Http;
using MyHttpClientProject.Builders;
using MyHttpClientProject.Extensions;
using Xunit;

namespace TestProject.Extensions
{
    public class RequestOptionsBuilderExtensionsTests
    {
        private readonly IRequestOptionsBuilder _builder;

        public RequestOptionsBuilderExtensionsTests()
        {
            _builder = new RequestOptionsBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri("http://google.com");
        }

        [Fact]
        public void SetConnectionHeader_Should_Set_ConnectionClose_When_True()
        {
            //Act
            var result = _builder
                .SetConnectionHeader(true)
                .Build();

            //Assert
            Assert.Contains(new KeyValuePair<string, string>("Connection", "close"), result.Headers);
        }

        [Fact]
        public void SetConnectionHeader_Should_Set_Connection_KeepAlive_When_False()
        {
            //Act
            var result = _builder
                .SetConnectionHeader(false)
                .Build();

            //Assert
            Assert.Contains(new KeyValuePair<string, string>("Connection", "keep-alive"), result.Headers);
        }

        [Theory]
        [InlineData(1.1)]
        [InlineData(double.MaxValue)]
        [InlineData(double.MinValue)]
        [InlineData(-1)]
        public void SetAcceptHeader_Should_ThrowException_When_Invalid_QFactor_value(double qFactor)
        {
            //Act and Assert
            Assert.Throws<ArgumentException>(() => _builder.SetAcceptHeader("mediaType", qFactor));
        }

        [Fact]
        public void SetAcceptHeader_Should_Set_Expected_Value_When_Valid_Parameters()
        {
            //Arrange
            var parameters = new Dictionary<string, double>
            {
                { "mediaType1", 0.5 },
                { "mediaType2", 1 },
                { "mediaType3", 0.8 }
            };

            string expected = "mediaType2, mediaType3;q=0.8, mediaType1;q=0.5";

            //Act
            var result = _builder
                .SetAcceptHeader(parameters)
                .Build();

            //Assert
            Assert.Contains(new KeyValuePair<string, string>("Accept", expected), result.Headers);
        }
    }
}
