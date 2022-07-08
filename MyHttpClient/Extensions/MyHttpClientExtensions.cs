using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyHttpClientProject.Builders;
using MyHttpClientProject.HttpBody;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Extensions
{
    public static class MyHttpClientExtensions
    {
        public static Task<HttpResponse> GetAsync(this IMyHttpClient client, string uri)
        {
            var builder = new RequestOptionsBuilder();

            var request = builder
                .SetMethod(HttpMethod.Get)
                .SetUri(uri)
                .Build();

            return client.GetResponseAsync(request);
        }

        public static async Task<HttpResponse> PostAsync<THttpBody>(this IMyHttpClient client, string uri, THttpBody body)
            where THttpBody : IHttpBody
        {
            var builder = new RequestOptionsBuilder();

            var request = builder
                .SetMethod(HttpMethod.Post)
                .SetUri(uri)
                .SetBody(body)
                .Build();

            return await client.GetResponseAsync(request);
        }

        public static async Task<string> PostWithStringResponseAsync<THttpBody>(this IMyHttpClient client, string uri, THttpBody body)
            where THttpBody : IHttpBody
        {
            var response = await PostAsync(client, uri, body);

            return Encoding.UTF8.GetString(response.ResponseBody.ToArray());
        }

        public static async Task<byte[]> PostWithByteArrayResponseAsync<THttpBody>(this IMyHttpClient client, string uri, THttpBody body)
            where THttpBody : IHttpBody
        {
            var response = await PostAsync(client, uri, body);

            return response.ResponseBody.ToArray();
        }

        public static async Task<Stream> PostWithStreamResponseAsync<THttpBody>(this IMyHttpClient client, string uri, THttpBody body)
            where THttpBody : IHttpBody
        {
            var response = await PostAsync(client, uri, body);

            return new MemoryStream(response.ResponseBody.ToArray());
        }
    }
}
