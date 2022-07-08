using System;
using System.IO;
using Moq;
using MyHttpClientProject.Services;
using MyHttpClientProject.Services.Interfaces;
using Xunit;

namespace TestProject.Services
{
    public class ConnectionTests
    {
        private readonly Mock<ITcpClient> _mockedClient;
        private readonly Mock<Stream> _mockedStream;
        private readonly IConnection _connection;

        public ConnectionTests()
        {
            _mockedStream = new Mock<Stream>();

            _mockedClient = new Mock<ITcpClient>();

            _mockedClient
                .Setup(x => x.GetStream())
                .Returns(_mockedStream.Object);
            

            _connection = new Connection(_mockedClient.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void SendAsync_Should_ThrowException_When_Address_Invalid(string address)
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connection.SendAsync(address, 1, new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void SendAsync_Should_ThrowException_When_Data_Null()
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connection.SendAsync("google.com", 1, null));
        }

        [Fact]
        public async void SendAsync_Should_Open_Connection_When_Not_Connected()
        {
            //Arrange
            _mockedClient
                .Setup(x => x.Connected(It.IsAny<string>(), It.IsAny<ushort>()))
                .Returns(false);

            //Act
            await _connection.SendAsync("google.com", 1, new byte[] { 1, 2, 3 });

            //Assert
            _mockedClient.Verify(x => x.OpenNewConnection("google.com", 1), Times.Once);
        }

        [Fact]
        public async void SendAsync_Should_Not_Open_Connection_When_Connected()
        {
            //Arrange
            _mockedClient
                .Setup(x => x.Connected(It.IsAny<string>(), It.IsAny<ushort>()))
                .Returns(true);

            //Act
            await _connection.SendAsync("google.com", 1, new byte[] { 1, 2, 3 });
            
            //Assert
            _mockedClient.Verify(x => x.OpenNewConnection("google.com", 1), Times.Never);
        }

        [Fact]
        public void ReadHeaders_And_ReadBody_Should_ThrowException_When_Stream_Can_Not_Read()
        {
            //Arrange
            _mockedStream
                .Setup(x => x.CanRead)
                .Returns(false);

            //Act and Assert
            Assert.Throws<InvalidOperationException>(() => _connection.ReadHeaders());
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
