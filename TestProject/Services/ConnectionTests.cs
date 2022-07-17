using System;
using System.Runtime.InteropServices;
using MyHttpClientProject.Services;
using MyHttpClientProject.Services.Interfaces;
using Xunit;

namespace TestProject.Services
{
    public class ConnectionTests
    {
        private const ushort ExamplePort = 1;
        private readonly IConnection _connection = new Connection();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void SendRequestAsync_Should_ThrowException_When_Address_Invalid(string address)
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connection.SendRequestAsync(address, ExamplePort, new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void SendAsync_Should_ThrowException_When_Data_Null()
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connection.SendRequestAsync("google.com", ExamplePort, null));
        }

        [Fact]
        public void ReadHeaders_Should_ThrowException_When_Not_Connected()
        {
            //Act and Assert
            Assert.Throws<InvalidOperationException>(() => _connection.ReadHeaders());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(int.MaxValue)]
        public void ReadBody_Should_ThrowException_When_NotConnected(int bodyLength)
        {
            //Act and Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _connection.ReadBodyAsync(bodyLength));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReadBody_Should_ThrowException_When_BodyLength_Invalid(int bodyLength)
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connection.ReadBodyAsync(bodyLength));
        }
    }
}
