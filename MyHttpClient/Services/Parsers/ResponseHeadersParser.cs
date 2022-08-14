using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Services.Parsers
{
    public static class ResponseHeadersParser
    {
        public static HttpResponse ParseFromBytes(byte[] data)
        {
            if (data == null || !data.Any())
            {
                throw new ArgumentException("Data should not be null or empty", nameof(data));
            }

            var dataStringArray = Encoding.UTF8.GetString(data).Split(Environment.NewLine);

            var statusLine = dataStringArray.First();

            var headers = dataStringArray
                .Skip(1)
                .TakeWhile(x => !string.IsNullOrWhiteSpace(x));

            var result = new HttpResponse
            {
                StatusCode = GetStatusCode(statusLine),
                Headers = GetHeaders(headers)
            };

            return result;
        }

        private static HttpStatusCode GetStatusCode(string firstLine)
        {
            var secondPart = firstLine
                .Split(' ')
                .Skip(1)
                .FirstOrDefault();

            if (!Enum.TryParse(secondPart, out HttpStatusCode statusCode))
            {
                throw new FormatException("Can't parse status code");
            }

            return statusCode;
        }

        private static IDictionary<string, string> GetHeaders(IEnumerable<string> headers)
        {
            var result = new Dictionary<string, string>();
            var regex = new Regex(@"(\S+):\s(\S+)");

            foreach (var line in headers)
            {
                var match = regex.Match(line);

                if (!match.Success)
                {
                    throw new FormatException($"Invalid header found: {line}");
                }

                result.Add(match.Groups[1].ToString(), match.Groups[2].ToString());
            }

            return result;
        }
    }
}
