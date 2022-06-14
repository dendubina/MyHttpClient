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
                .GetResult();

            return client.GetResponseAsync(request);
        }

        public static async Task<HttpResponse> PostAsync<TReq>(this IMyHttpClient client, string uri, TReq body)
            where TReq : HttpBodyBase
        {
            var builder = new RequestOptionsBuilder();

            var request = builder
                .SetMethod(HttpMethod.Post)
                .SetUri(uri)
                .AddBody(body)
                .GetResult();

            return await client.GetResponseAsync(request);
        }

        public static async Task<string> PostWithStringResponseASync<TReq>(this IMyHttpClient client, string uri, TReq body)
            where TReq : HttpBodyBase
        {
            var response = await PostAsync(client, uri, body);

            return Encoding.UTF8.GetString(response.ResponseBody.ToArray());
        }

        public static async Task<byte[]> PostWithByteArrayResponseAsync<TReq>(this IMyHttpClient client, string uri, TReq body)
            where TReq : HttpBodyBase
        {
            var response = await PostAsync(client, uri, body);

            return response.ResponseBody.ToArray();
        }

        public static async Task<MemoryStream> PostWithStreamResponseAsync<TReq>(this IMyHttpClient client, string uri, TReq body)
            where TReq : HttpBodyBase
        {
            var response = await PostAsync(client, uri, body);

            return new MemoryStream(response.ResponseBody.ToArray());
        }
    }
}
