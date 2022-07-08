using System;
using System.Text;
using MyHttpClientProject.HttpBody;
using Xunit;
using Xunit.Abstractions;

namespace TestProject.HttpBody
{
    public class MultipartFormDataBodyTests
    {
        private readonly MultipartFormDataBody _multipartBody = new();

        [Fact]
        public void Add_Should_ThrowException_When_Body_Parameter_Null()
        {
            //Act and Assert
            Assert.Throws<ArgumentNullException>(() => _multipartBody.Add(null, "fieldName"));
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
            //Act and Assert
            Assert.Throws<ArgumentException>(() => _multipartBody.Add(new StringBody("content"), fieldName));
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        [InlineData("name\r")]
        [InlineData("\nname\r")]
        [InlineData("\r\n")]
        public void Add_Should_ThrowException_When_FileName_Not_Valid(string fileName)
        {
            //Act and Assert
            Assert.Throws<ArgumentException>(() => _multipartBody.Add(new StringBody("content"), fileName));
        }

        [Fact]
        public void GetContent_Should_ThrowException_When_No_Body_Elements()
        {
            //Act and Assert
            Assert.Throws<InvalidOperationException>(() => _multipartBody.GetContent());
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

            //Act and Assert
            Assert.Equal(expected, Encoding.UTF8.GetString(_multipartBody.GetContent()));
        }
    }
}
