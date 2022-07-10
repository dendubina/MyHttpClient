using System;
using MyHttpClientProject.HttpBody;
using Xunit;

namespace TestProject.HttpBody
{
    public class ByteArrayBodyTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(new byte[]{})]
        public void Constructor_Should_ThrowException_When_Content_Is_Null_Or_Empty(byte[] content)
        {
            //Act and Assert
            Assert.Throws<ArgumentException>(() => new ByteArrayBody(content));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("mediaType\r")]
        [InlineData("\nmediaType\r")]
        public void Constructor_Should_ThrowException_When_MediaType_Is_Invalid(string mediaType)
        {
            //Arrange
            var content = new byte[] {1, 2, 3, 4, 5};

            //Act and Assert
            Assert.Throws<ArgumentException>(() => new ByteArrayBody(content, mediaType));
        }

        [Theory]
        [InlineData("image/jpeg")]
        [InlineData("audio/ogg")]
        [InlineData("video/mp4")]
        public void Constructor_Should_Set_MediaType_When_Value_Valid(string mediaType)
        {
            //Arrange
            var content = new byte[] { 1, 2, 3 };

            //Act
            var body = new ByteArrayBody(content, mediaType);

            //Assert
            Assert.Equal(mediaType, body.MediaType);
        }

        [Theory]
        [InlineData(new byte[]{ 1 })]
        [InlineData(new byte[]{ 1, 2, 3, 4, 5 })]
        public void GetContent_Should_Return_Expected_Content(byte[] content)
        {
            //Arrange
            var body = new ByteArrayBody(content);

            //Act and Assert
            Assert.Equal(content, body.GetContent());
        }
    }
}
