using System;
using FluentAssertions;
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
            //Act
            Action act = () => new ByteArrayBody(content);

            //Assert
            act.Should().Throw<ArgumentException>();
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

            //Act 
            Action act = () => new ByteArrayBody(content, mediaType);

            //Assert
            act.Should().Throw<ArgumentException>();
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
            mediaType.Should().Be(body.MediaType);
        }

        [Theory]
        [InlineData(new byte[]{ 1 })]
        [InlineData(new byte[]{ 1, 2, 3, 4, 5 })]
        public void GetContent_Should_Return_Expected_Content(byte[] content)
        {
            //Arrange
            var body = new ByteArrayBody(content);

            //Act
            var actual = body.GetContent();

            //Assert
            actual.Should().Equal(content);
        }
    }
}
