using System.Threading;
using System.Threading.Tasks;
using MyHttpClientProject.Models;
using MyHttpClientProject.Parsers;
using MyHttpClientProject.WebConnection;

namespace MyHttpClientProject
{
    public class MyHttpClient : IMyHttpClient
    {
        private readonly IWebConnection _webConnection;
        private static readonly SemaphoreSlim SemaphoreSlim = new(1,1);

        public MyHttpClient() : this(new TcpClientConnection())
        {

        }

        public MyHttpClient(IWebConnection webClient)
        {
            _webConnection = webClient;
        }

        public async Task<HttpResponse> GetResponseAsync(RequestOptions options)
        {
            var requestBytes = RequestParser.ParseToHttpRequestBytes(options);

            await SemaphoreSlim.WaitAsync();

            try
            {
                var response = await _webConnection.SendAsync(options.Uri.Host, options.Port, requestBytes);

                var parsedResponse = ResponseParser.ParseFromBytes(response);

                if (parsedResponse.ResponseHeaders.TryGetValue("Connection", out string value) && value.ToLowerInvariant() == "close")
                {
                    _webConnection.Dispose();
                }

                return parsedResponse;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public void Dispose()
        {
            _webConnection.Dispose();
        }
    }
}
