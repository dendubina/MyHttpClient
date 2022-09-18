using System;
using System.Text;
using FluentAssertions;
using MyHttpClientProject.HttpBody;
using Xunit;

namespace MyHttpClientProject.Tests.HttpBody
{
    public class StringBodyTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Constructor_Should_ThrowException_When_Content_Is_Not_Valid(string content)
        {
            //Act
            Action act = () => new StringBody(content);

            //Assert
            act.Should().Throw<ArgumentException>();
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
            //Act
            Action act = () => new StringBody("content", Encoding.UTF8, mediaType);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_Should_Set_Default_MediaType_And_Encoding_When_Not_Specified()
        {
            //Arrange
            string defaultMediaType = "text/plain; charset=utf-8";

            //Act
            var actualBody = new StringBody("content");

            //Assert
            actualBody.MediaType.Should().Be(defaultMediaType);
        }

        [Fact]
        public void Constructor_Should_Set_MediaType_And_CharSet()
        {
            //Arrange
            var encoding = Encoding.UTF8;
            string mediaType = "application/json";

            //Act
            var actualBody = new StringBody("content", encoding, mediaType);

            //Assert
            actualBody.MediaType.Should().Be($"{mediaType}; charset={encoding.BodyName}");
        }

        [Fact]
        public void GetContent_Should_Return_Expected_Content()
        {
            //Arrange
            string expectedContent = "content";
            var body = new StringBody(expectedContent);

            //Act
            var actualContent = body.GetContent();

            //Assert
            actualContent.Should().Equal(Encoding.UTF8.GetBytes(expectedContent));
        }
    }
}
