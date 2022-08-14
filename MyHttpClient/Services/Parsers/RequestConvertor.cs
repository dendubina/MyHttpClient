using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Services.Parsers
{
    public static class RequestConvertor
    {
        public static IEnumerable<byte> ParseToHttpRequestBytes(RequestOptions options)
        {
            ValidateRequiredValues(options);

            var requestLineWithHeaders = new StringBuilder();

            requestLineWithHeaders.AppendLine($"{options.Method} {options.Uri.AbsoluteUri} HTTP/1.1");

            foreach (var (name, value) in options.Headers)
            {
                requestLineWithHeaders.AppendLine($"{name}: {value}");
            }

            requestLineWithHeaders.AppendLine();

            var result = Encoding.UTF8.GetBytes(requestLineWithHeaders.ToString());

            return options.Body?.GetContent() != null ? result.Concat(options.Body.GetContent()) : result;
        }

        private static void ValidateRequiredValues(RequestOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Request options must not be null");
            }

            if (options.Method == null)
            {
                throw new ArgumentNullException(nameof(options), "Method must not be null");
            }

            if (options.Uri == null)
            {
                throw new ArgumentNullException(nameof(options), "Uri must not be null");
            }

            if (options.Headers == null)
            {
                throw new ArgumentNullException(nameof(options), "Headers must not be null");
            }
        }
    }
}
