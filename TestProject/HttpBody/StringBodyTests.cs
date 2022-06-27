using System;
using System.Text;
using MyHttpClientProject.HttpBody;
using Xunit;

namespace TestProject.HttpBody
{
    public class StringBodyTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Constructor_Should_ThrowException_When_Content_Is_Not_Valid(string content)
        {
            //Act and Assert
            Assert.Throws<ArgumentException>(() => new StringBody(content));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("mediaType\r")]
        [InlineData("\nmediaType\r")]
        [InlineData("\r\n")]
        public void Constructor_Should_ThrowException_When_MediaType_Is_Invalid(string mediaType)
        {
            //Act and Assert
            Assert.Throws<ArgumentException>(() => new StringBody("content", Encoding.UTF8, mediaType));
        }

        [Fact]
        public void Constructor_Should_Set_Default_MediaType_And_Encoding_When_Not__Specified()
        {
            //Arrange
            string defaultMediaType = "text/plain; charset=utf-8";

            //Act
            var body = new StringBody("content");

            //Assert
            Assert.Equal(defaultMediaType, body.MediaType);
        }

        [Fact]
        public void Constructor_Should_Set_MediaType_And_CharSet()
        {
            //Arrange
            var encoding = Encoding.UTF8;
            string mediaType = "application/json";

            //Act
            var body = new StringBody("content", encoding, mediaType);

            //Assert
            Assert.Equal($"{mediaType}; charset={encoding.BodyName}", body.MediaType);
        }

        [Fact]
        public void GetContent_Should_Return_Expected_Content()
        {
            //Arrange
            string content = "content";
            var body = new StringBody(content);

            //Act
            var actual = body.GetContent();

            //Assert
            Assert.Equal(Encoding.UTF8.GetBytes(content), actual);
        }
    }
}
