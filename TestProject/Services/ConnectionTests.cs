using System;
using MyHttpClientProject.Services;
using MyHttpClientProject.Services.Interfaces;
using Xunit;

namespace TestProject.Services
{
    public class ConnectionTests
    {
        private readonly IConnection _connection = new Connection();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void SendRequestAsync_Should_ThrowException_When_Address_Invalid(string address)
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connection.SendRequestAsync(address, 1, new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void SendAsync_Should_ThrowException_When_Data_Null()
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connection.SendRequestAsync("google.com", 1, null));
        }

        [Fact]
        public void ReadHeaders_Should_ThrowException_When_Not_Connected()
        {
            //Act and Assert
            Assert.Throws<InvalidOperationException>(() => _connection.ReadHeaders());
        }

        [Fact]
        public void ReadBody_Should_ThrowException_When_NotConnected()
        {
            //Act and Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _connection.ReadBody(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReadBody_Should_ThrowException_When_BodyLength_Invalid(int bodyLength)
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connection.ReadBody(bodyLength));
        }
    }
}
