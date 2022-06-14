using System.Collections.Generic;
using System.Text;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Parsers
{
    public static class RequestParser
    {
        public static byte[] ParseToHttpRequestBytes(RequestOptions options)
        {
            var headers = new StringBuilder();
            var result = new List<byte>();

            headers.AppendLine($"{options.Method} {options.Uri.AbsoluteUri} HTTP/1.1");

            foreach (var (name, value) in options.Headers)
            {
                headers.AppendLine($"{name}: {value}");
            }

            headers.AppendLine();

            result.AddRange(Encoding.UTF8.GetBytes(headers.ToString()));

            if (options.Body != null)
            {
                result.AddRange(options.Body);
            }

            return result.ToArray();
        }
    }
}
