using System;
using System.Text;
using FluentAssertions;
using MyHttpClientProject.HttpBody;
using Xunit;

namespace MyHttpClientProject.Tests.HttpBody
{
    public class MultipartFormDataBodyTests
    {
        private readonly MultipartFormDataBody _multipartBody = new();


        [Fact]
        public void Constructor_Should_Set_Expected_Media_Type()
        {
            //Arrange
            var expected = $"multipart/form-data;boundary=\"{_multipartBody.Boundary}\"";

            //Assert
            _multipartBody.MediaType.Should().Be(expected);
        }

        [Fact]
        public void Add_Should_ThrowException_When_Body_Parameter_Null()
        {
            //Act 
            Action act = () => _multipartBody.Add(null, "fieldName");

            //Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("name\r")]
        [InlineData("\nname\r")]
        [InlineData("\r\n")]
        public void Add_Should_ThrowException_When_FieldName_Not_Valid(string fieldName)
        {
            //Act 
            Action act = () => _multipartBody.Add(new StringBody("content"), fieldName);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        [InlineData("name\r")]
        [InlineData("\nname\r")]
        [InlineData("\r\n")]
        public void Add_Should_ThrowException_When_FileName_Not_Valid(string fileName)
        {
            //Act
            Action act = () => _multipartBody.Add(new StringBody("content"), "fieldName", fileName);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetContent_Should_ThrowException_When_No_Body_Elements()
        {
            //Act
            Action act = () => _multipartBody.GetContent();

            //Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetContent_Should_Return_Expected_Content()
        {
            //Arrange
            var firstContent = "firstContent";
            var firstContentFieldName = "firstContentName";
            var firstContentPart = new StringBody("firstContent");

            var secondContent = "secondContent";
            var secondContentFieldName = "secondContentName";
            var secondContentFileName = "secondContentFileName";
            var secondContentPart = new StringBody(secondContent);

            _multipartBody.Add(firstContentPart, firstContentFieldName);
            _multipartBody.Add(secondContentPart, secondContentFieldName, secondContentFileName);

            string expected =
                $"--{_multipartBody.Boundary}{Environment.NewLine}" +
                $"Content-Disposition: form-data; name=\"{firstContentFieldName}\"{Environment.NewLine}" +
                $"Content-Type: text/plain; charset=utf-8{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"{firstContent}{Environment.NewLine}" +

                $"--{_multipartBody.Boundary}{Environment.NewLine}" +
                $"Content-Disposition: form-data; name=\"{secondContentFieldName}\"; filename=\"{secondContentFileName}\"{Environment.NewLine}" +
                $"Content-Type: text/plain; charset=utf-8{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"{secondContent}{Environment.NewLine}" +
                $"--{_multipartBody.Boundary}--";

            //Act
            var actual = Encoding.UTF8.GetString(_multipartBody.GetContent());

            // Assert
            actual.Should().Be(expected);
        }
    }
}
