using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyHttpClientProject.Models;
using MyHttpClientProject.Services;
using MyHttpClientProject.Services.Interfaces;
using MyHttpClientProject.Services.Parsers;

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

            _connection.ReceiveTimeout = options.ReceiveTimeout;
            _connection.SendTimeout = options.SendTimeout;

            try
            {

                await _connection.SendRequestAsync(options.Uri.Host, options.Port, requestBytes);

                var headers = _connection.ReadHeaders();

                var parsedResponse = ResponseParser.ParseFromBytes(headers.ToArray());

                if (parsedResponse.ResponseHeaders.TryGetValue("Content-Length", out string lengthString) && int.TryParse(lengthString, out int parsedLength))
                {
                    parsedResponse.ResponseBody = await _connection.ReadBodyAsync(parsedLength);
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
