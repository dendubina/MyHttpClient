﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using MyHttpClientProject.HttpBody;
using MyHttpClientProject.Models;
using MyHttpClientProject.Services.Parsers;
using Xunit;

namespace MyHttpClientProject.Tests.Services.Parsers
{
    public class RequestParserTests
    {
        private readonly RequestOptions _options;

        public RequestParserTests()
        {
            _options = new RequestOptions
            {
                Method = HttpMethod.Get,

                Uri = new Uri("http://google.com"),

                Headers = new Dictionary<string, string>
                {
                    {"Host", "google.com"}
                },
            };
        }

        [Fact]
        public void ParseToHttpRequestBytes_Should_ThrowException_When_Parameter_Null()
        {
            //Act
            Action act = () => RequestConvertor.ParseToHttpRequestBytes(null);

            //Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParseToHttpRequestBytes_Should_ThrowException_When_Method_Null()
        {
            //Arrange
            _options.Method = null;

            //Act
            Action act = () => RequestConvertor.ParseToHttpRequestBytes(_options);

            //Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParseToHttpRequestBytes_Should_ThrowException_When_Uri_Null()
        {
            //Arrange
            _options.Uri = null;

            //Act
            Action act = () => RequestConvertor.ParseToHttpRequestBytes(_options);

            //Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParseToHttpRequestBytes_Should_ThrowException_When_Headers_Null()
        {
            //Arrange
            _options.Headers = null;

            //Act
            Action act = () => RequestConvertor.ParseToHttpRequestBytes(_options);

            //Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParseToHttpRequestBytes_Should_ReturnExpectedByteArray_When_HasBody()
        {
            //Arrange
            const string body = "<HTML>body</HTML>";

            string expected = $"GET http://google.com/ HTTP/1.1{Environment.NewLine}" +
                              $"Host: google.com{Environment.NewLine}" +
                              $"Content-Length: {Encoding.UTF8.GetByteCount(body)}{Environment.NewLine}" +
                              $"Content-Type: text/plain; charset=utf-8{Environment.NewLine}" +
                              $"{Environment.NewLine}" +
                              $"{body}";

            _options.Body = new StringBody(body);
            _options.Headers.Add("Content-Length", Encoding.UTF8.GetByteCount(body).ToString());
            _options.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //Act
            var actual = RequestConvertor.ParseToHttpRequestBytes(_options);

            //Assert
            actual.Should().Equal(Encoding.UTF8.GetBytes(expected));
        }

        [Fact]
        public void ParseToHttpRequestBytes_Should_ReturnExpectedByteArray_When_NoBody()
        {
            //Arrange
            string expected = $"GET http://google.com/ HTTP/1.1{Environment.NewLine}" +
                              $"Host: google.com{Environment.NewLine}" +
                              $"{Environment.NewLine}";

            //Act
            var actual = RequestConvertor.ParseToHttpRequestBytes(_options);

            //Assert
            actual.Should().Equal(Encoding.UTF8.GetBytes(expected));
        }
    }
}
