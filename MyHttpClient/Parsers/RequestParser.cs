using System.Linq;
using System.Text;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Parsers
{
    public static class RequestParser
    {
        public static byte[] ParseToHttpRequestBytes(RequestOptions options)
        {
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
