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
    }
}
