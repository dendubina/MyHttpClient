using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyHttpClientProject.Services;
using MyHttpClientProject.Services.Interfaces;
using Xunit;

namespace TestProject.Services
{
    public class ConnectionHandlerTests
    {
        private const ushort SomeTestPort = 1;
        private const string SomeTestAddress = "google.com";

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
            //Act
            Func<Task> act = () => _connectionHandler.SendAsync(address, SomeTestPort, new byte[] { 1, 2, 3 });

            //Assert
            act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void SendAsync_Should_ThrowException_When_Data_Null()
        {
            //Act
            Func<Task> act = () => _connectionHandler.SendAsync(SomeTestAddress, SomeTestPort, null);

            //Assert
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void SendAsync_Should_OpenConnection_When_Not_Connected()
        {
            //Arrange
            var exampleData = new byte[] { 1, 2, 3 };

            _mockClient
                .Setup(x => x.IsConnected(It.IsAny<string>(), It.IsAny<ushort>()))
                .Returns(false);

            //Act
            _connectionHandler.SendAsync(SomeTestAddress, SomeTestPort, exampleData);

            //Assert
            _mockClient.Verify(x => 
                x.OpenConnection(It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
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
            await _connectionHandler.SendAsync(SomeTestAddress, SomeTestPort, data);

            //Assert
            stream.ToArray().Should().Equal(data);
        }

        [Fact]
        public void ReadHeaders_Should_ThrowException_When_Cannot_Read()
        {
            //Arrange
            var stream = new MemoryStream();

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            stream.Close();

            //Act
            Action act = () => _connectionHandler.ReadHeaders();

            //Assert
            act.Should().Throw<InvalidOperationException>();
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

            //Act
            Action act = () => _connectionHandler.ReadHeaders();

            //Assert
            act.Should().Throw<InvalidOperationException>();
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
            var actualHeaders = _connectionHandler.ReadHeaders();

            //Assert
            actualHeaders.Should().Equal(expectedHeaders);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(int.MaxValue)]
        public void ReadBodyAsync_Should_ThrowException_When_Cannot_Read(int bodyLength)
        {
            //Arrange
            var stream = new MemoryStream();

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            stream.Close();

            //Act
            Func<Task> act = () => _connectionHandler.ReadBodyAsync(bodyLength);

            //Assert
            act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReadBodyAsync_Should_ThrowException_When_BodyLength_Invalid(int bodyLength)
        {
            //Act
            Func<Task> act = () => _connectionHandler.ReadBodyAsync(bodyLength);

            //Assert
            act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void ReadBodyAsync_Should_ThrowException_When_Parameter_Not_Equal_Actual_Response()
        {
            //Arrange
            var responseData = new byte[] { 1, 2, 3 };

            using var stream = new MemoryStream(responseData);

            int invalidLength = 100;

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            //Act
            Func<Task> act = () => _connectionHandler.ReadBodyAsync(invalidLength);

            //Assert
            act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async void ReadBodyAsync_Should_Return_Expected_Content_When_Valid_Response_And_Parameter()
        {
            //Arrange
            var expectedResponseData = Encoding.UTF8.GetBytes("example content");

            using var stream = new MemoryStream(expectedResponseData);

            _mockClient
                .Setup(x => x.GetStream())
                .Returns(stream);

            //Act
            var actual = await _connectionHandler.ReadBodyAsync(expectedResponseData.Length);

            //Assert
            actual.Should().Equal(expectedResponseData);
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
