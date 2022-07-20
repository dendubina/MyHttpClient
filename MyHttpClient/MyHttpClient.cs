using System;
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
        private readonly IConnectionHandler _connection;
        private static readonly SemaphoreSlim SemaphoreSlim = new(1,1);

        public MyHttpClient() : this(new ConnectionHandler())
        {

        }

        public MyHttpClient(IConnectionHandler connection)
        {
            _connection = connection;
        }

        public async Task<HttpResponse> GetResponseAsync(RequestOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Request options must not be null");
            }

            var requestBytes = RequestParser.ParseToHttpRequestBytes(options);

            await SemaphoreSlim.WaitAsync();

            _connection.ReceiveTimeout = options.ReceiveTimeout;
            _connection.SendTimeout = options.SendTimeout;

            try
            {

                await _connection.SendAsync(options.Uri.Host, options.Port, requestBytes);

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
