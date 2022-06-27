using System;
using System.Text;
using MyHttpClientProject.HttpBody;
using Xunit;

namespace TestProject.HttpBody
{
    public class MultipartFormDataBodyTests
    {
        private readonly MultipartFormDataBody _multipartBody = new();

        private struct HttpBodyPart
        {
            public string Content { get; init; }
            public string FieldName { get; init; }
            public string FileName { get; init; }
        }

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
        public void GetContent_Should_Return_Expected_Content() // ну а как))
        {
            //Arrange
            var body = new MultipartFormDataBody();

            var firstDataPart = new HttpBodyPart
            {
                Content = "firstContent",
                FieldName = "firstFieldName",
            };

            var secondDataPart = new HttpBodyPart()
            {
                Content = "secondContent",
                FieldName = "secondFieldName",
                FileName = "secondFileName",
            };

            body.Add(new StringBody(firstDataPart.Content), firstDataPart.FieldName);
            body.Add(new StringBody(secondDataPart.Content), secondDataPart.FieldName, secondDataPart.FileName);

            string expected =
                $"--{body.Boundary}" + Environment.NewLine +
                $"Content-Disposition: form-data; name=\"{firstDataPart.FieldName}\"" + Environment.NewLine +
                "Content-Type: text/plain; charset=utf-8" + Environment.NewLine +
                Environment.NewLine +

                firstDataPart.Content + Environment.NewLine +

                $"--{body.Boundary}" + Environment.NewLine +
                $"Content-Disposition: form-data; name=\"{secondDataPart.FieldName}\"; filename=\"{secondDataPart.FileName}\"" +
                Environment.NewLine +
                "Content-Type: text/plain; charset=utf-8" + Environment.NewLine +
                Environment.NewLine +

                secondDataPart.Content + Environment.NewLine +
                $"--{body.Boundary}--";

            //Act and Assert
            Assert.Equal(Encoding.UTF8.GetBytes(expected), body.GetContent());
        }
    }
}
