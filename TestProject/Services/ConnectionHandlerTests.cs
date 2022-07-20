using System;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using MyHttpClientProject.Services;
using MyHttpClientProject.Services.Interfaces;
using Xunit;

namespace TestProject.Services
{
    public class ConnectionHandlerTests
    {
        private const ushort ExamplePort = 1;
        private const string ExampleAddress = "google.com";

        private readonly Mock<IClient> _mockClient;
        private readonly IConnectionHandler _connectionHandler;

        public ConnectionHandlerTests()
        {
            _mockClient = new Mock<IClient>();
            _connectionHandler = new ConnectionHandler(_mockClient.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void SendAsync_Should_ThrowException_When_Address_Invalid(string address)
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connectionHandler.SendAsync(address, ExamplePort, new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void SendAsync_Should_ThrowException_When_Data_Null()
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _connectionHandler.SendAsync(ExampleAddress, ExamplePort, null));
        }

        [Fact]
        public void SendAsync_Should_OpenConnection_When_Not_Connected()
        {
            //Arrange
            var exampleData = new byte[] { 1, 2, 3 };

            _mockClient
                .Setup(x => x.Connected(It.IsAny<string>(), It.IsAny<ushort>()))
                .Returns(false);

            //Act
            _connectionHandler.SendAsync(ExampleAddress, ExamplePort, exampleData);

            //Assert
            _mockClient.Verify(x => 
                x.OpenNewConnection(It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async void SendAsync_Should_Write_Data_To_Stream()
        {
            //Arrange
            var data = new byte[] { 1, 2, 3 };
            using var stream = new MemoryStream();

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            //Act 
            await _connectionHandler.SendAsync(ExampleAddress, ExamplePort, data);

            //Assert
            Assert.Equal(data, stream.ToArray());
        }

        [Fact]
        public void ReadHeaders_Should_ThrowException_When_Cant_Read()
        {
            //Arrange
            var stream = new MemoryStream();

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            stream.Close();

            //Act and Assert
            Assert.Throws<InvalidOperationException>(() => _connectionHandler.ReadHeaders());
        }

        [Theory]
        [InlineData("invalid data")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\r\n\r\n")]
        [InlineData("\r\n")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void ReadHeaders_Should_ThrowException_When_Invalid_Response(string response)
        {
            //Arrange
            var responseData = Encoding.UTF8.GetBytes(response);
            using var stream = new MemoryStream(responseData);

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            //Act and Assert
            Assert.Throws<InvalidOperationException>(() => _connectionHandler.ReadHeaders());
        }

        [Fact]
        public void ReadHeaders_Should_Return_Expected_Data_When_Valid_Response()
        {
            //Arrange
            var expectedHeaders = Encoding.UTF8.GetBytes(
                  $"HTTP/1.1 200 OK{Environment.NewLine}" +
                    $"Server: Apache{Environment.NewLine}" +
                    $"{Environment.NewLine}");

            var responseBody = Encoding.UTF8.GetBytes("example body");

            var responseData = expectedHeaders.Concat(responseBody).ToArray();

            using var stream = new MemoryStream(responseData);

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            //Act
            var actual = _connectionHandler.ReadHeaders();

            //Assert
            Assert.Equal(expectedHeaders, actual);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(int.MaxValue)]
        public void ReadBodyAsync_Should_ThrowException_When_Cant_Read(int bodyLength)
        {
            //Arrange
            var stream = new MemoryStream();

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            stream.Close();

            //Act and Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _connectionHandler.ReadBodyAsync(bodyLength));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReadBodyAsync_Should_ThrowException_When_BodyLength_Invalid(int bodyLength)
        {
            //Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _connectionHandler.ReadBodyAsync(bodyLength));
        }

        [Fact]
        public async void ReadBodyAsync_Should_ThrowException_When_Parameter_Doesnt_Equal_Actual_Response()
        {
            //Arrange
            var responseData = new byte[] { 1, 2, 3 };

            using var stream = new MemoryStream(responseData);

            int invalidLength = 100;

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);
           
            //Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _connectionHandler.ReadBodyAsync(invalidLength));
        }

        [Fact]
        public async void ReadBodyAsync_Should_Return_Expected_Content_When_Valid_Response_And_Parameter()
        {
            //Arrange
            var responseData = Encoding.UTF8.GetBytes("example content");

            using var stream = new MemoryStream(responseData);

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            //Act
            var actual = await _connectionHandler.ReadBodyAsync(responseData.Length);

            //Assert
            Assert.Equal(responseData, actual);
        }

        [Fact]
        public void CloseConnection_Should_Call_Client_Close()
        {
            //Act
            _connectionHandler.CloseConnection();

            //Assert
            _mockClient.Verify(x => x.CloseConnection(), Times.Once);
        }
    }
}
