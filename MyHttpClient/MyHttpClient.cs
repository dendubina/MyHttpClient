using System;
using System.Collections.Generic;
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
        private readonly IDataHandler _connection;
        private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public MyHttpClient() : this(new DataHandler())
        {

        }

        public MyHttpClient(IDataHandler connection)
        {
            _connection = connection;
        }

        public async Task<HttpResponse> SendAsync(RequestOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Request options must not be null");
            }

            var requestBytes = RequestConvertor.ParseToHttpRequestBytes(options);

            await _semaphoreSlim.WaitAsync();

            try
            {
                _connection.ReceiveTimeout = options.ReceiveTimeout;
                _connection.SendTimeout = options.SendTimeout;

                await _connection.SendAsync(options.Uri.Host, options.Port, requestBytes);

                var headers = _connection.ReadHeaders();

                var parsedResponse = ResponseHeadersParser.ParseFromBytes(headers.ToArray());

                var caseInsensitiveDictionary = new Dictionary<string, string>(parsedResponse.Headers, StringComparer.OrdinalIgnoreCase);

                if (caseInsensitiveDictionary.TryGetValue("Content-Length", out var lengthString) && int.TryParse(lengthString, out int parsedLength))
                {
                    var body = await _connection.ReadBodyAsync(parsedLength);
                    parsedResponse.Body = body.ToArray();
                }

                if (caseInsensitiveDictionary.TryGetValue("Connection", out string value) && value is not null && value.ToLowerInvariant() == "close")
                {
                    _connection.CloseConnection();
                }

                return parsedResponse;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void Dispose()
        {
            _connection.CloseConnection();
        }
    }
}
