using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyHttpClientProject.Models;
using MyHttpClientProject.Parsers;
using MyHttpClientProject.Services;
using MyHttpClientProject.Services.Interfaces;

namespace MyHttpClientProject
{
    public class MyHttpClient : IMyHttpClient
    {
        private readonly IConnection _connection;
        private static readonly SemaphoreSlim SemaphoreSlim = new(1,1);

        public MyHttpClient() : this(new Connection())
        {

        }

        public MyHttpClient(IConnection connection)
        {
            _connection = connection;
        }

        public async Task<HttpResponse> GetResponseAsync(RequestOptions options)
        {
            var requestBytes = RequestParser.ParseToHttpRequestBytes(options);

            await SemaphoreSlim.WaitAsync();

            try
            {

                await _connection.SendAsync(options.Uri.Host, options.Port, requestBytes);

                var headers = _connection.ReadHeaders();

                var parsedResponse = ResponseParser.ParseFromBytes(headers.ToArray());

                if (parsedResponse.ResponseHeaders.TryGetValue("Content-Length", out string lengthString) && int.TryParse(lengthString, out int parsedLength))
                {
                    parsedResponse.ResponseBody = await _connection.ReadBody(parsedLength);
                }

                if (parsedResponse.ResponseHeaders.TryGetValue("Connection", out string value) && value.ToLowerInvariant() == "close")
                {
                    _connection.CloseConnection();
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
            _connection.CloseConnection();
        }
    }
}
