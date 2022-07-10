using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Services.Parsers
{
    public static class ResponseParser
    {
        public static HttpResponse ParseFromBytes(byte[] data)
        {
            if (data == null || !data.Any())
            {
                throw new ArgumentException("Data should not be null or empty", nameof(data));
            }

            var dataStringArray = Encoding.UTF8.GetString(data).Split(Environment.NewLine);

            var result = new HttpResponse
            {
                StatusCode = GetStatusCode(dataStringArray.First()),
                ResponseHeaders = GetHeaders(dataStringArray)
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

        private static IDictionary<string, string> GetHeaders(IEnumerable<string> data)
        {
            var result = new Dictionary<string, string>();

            var headers = data
                .Skip(1)
                .TakeWhile(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            if (!headers.Any())
            {
                throw new FormatException("No headers found");
            }

            foreach (var line in headers)
            {
                int separatorIndex = line.IndexOf(':', StringComparison.Ordinal);

                if (separatorIndex <= 0 || line.Length <= separatorIndex + 2)
                {
                    throw new FormatException($"Invalid header found: {line}");
                }

                var name = line[..separatorIndex].Trim();
                var value = line[(separatorIndex + 2)..].Trim();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                {
                    throw new FormatException($"Invalid header found: {line}");
                }

                result.Add(name, value);
            }

            return result;
        }
    }
}
