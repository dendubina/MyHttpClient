using System;
using System.Linq;
using System.Text;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Services.Parsers
{
    public static class RequestParser
    {
        public static byte[] ParseToHttpRequestBytes(RequestOptions options)
        {
            if (options.Method == null)
            {
                throw new NullReferenceException("Method must not be null");
            }

            if (options.Uri == null)
            {
                throw new NullReferenceException("Uri must not be null");
            }

            if (options.Headers == null)
            {
                throw new NullReferenceException("Headers must not be null");
            }

            var headers = new StringBuilder();

            headers.AppendLine($"{options.Method} {options.Uri.AbsoluteUri} HTTP/1.1");

            foreach (var (name, value) in options.Headers)
            {
                headers.AppendLine($"{name}: {value}");
            }

            headers.AppendLine();

            var result = Encoding.UTF8.GetBytes(headers.ToString());

            return options.Body is null ? result : result.Concat(options.Body.GetContent()).ToArray();
        }
    }
}
