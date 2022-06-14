using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using MyHttpClientProject.Exceptions;
using MyHttpClientProject.Extensions;
using MyHttpClientProject.HttpBody;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Builders
{
    public class RequestOptionsBuilder : IRequestOptionsBuilder
    {
        private RequestOptions _options = new();
        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        public IRequestOptionsBuilder SetUri(string uri)
        {
            _options.Uri = new Uri(uri);

            return this;
        }

        public IRequestOptionsBuilder SetMethod(HttpMethod method)
        {
            _options.Method = method;

            return this;
        }

        public IRequestOptionsBuilder AddHeader(string name, string value)
        {
            if (name.ContainsNewLine() || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid header name format", nameof(name));
            }

            if (value.ContainsNewLine() || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Invalid header value format", nameof(value));
            }

            _options.Headers ??= new Dictionary<string, string>();

            _options.Headers.Add(name, value);

            return this;
        }

        public IRequestOptionsBuilder AddBody(HttpBodyBase body)
        {
            _options.Headers ??= new Dictionary<string, string>();

            if (body.MediaType != null)
            {
                _options.Headers.Add("Content-Type", body.MediaType);
            }

            if (body is not MultipartFormDataBody formDataBody)
            {
                _options.Headers.Add("Content-Length", body.BufferedContent.Length.ToString());
                _options.Body = body.BufferedContent;
            }
            else
            {
                var result = new List<byte>();
                var last = formDataBody.Contents.Last();

                _options.Headers["Content-Type"] = formDataBody.MediaType + $";boundary=\"{formDataBody.Boundary}\"";

                foreach (var (name, item) in formDataBody.Contents)
                {
                    StringBuilder header = new();

                    header.AppendLine($"--{formDataBody.Boundary}");
                    header.AppendLine($"Content-Disposition: form-data; name=\"{name}\"");

                    if (item.MediaType != null)
                    {
                        header.AppendLine($"Content-Type: {item.MediaType}");
                    }

                    header.AppendLine();

                    result.AddRange(_defaultEncoding.GetBytes(header.ToString()));

                    result.AddRange(item.BufferedContent);

                    result.AddRange(item.Equals(last.Value)
                        ? _defaultEncoding.GetBytes($"--{formDataBody.Boundary}--")
                        : _defaultEncoding.GetBytes(Environment.NewLine));
                }

                _options.Headers.Add("Content-Length", result.Count.ToString());
                _options.Body = result.ToArray();
            }

            return this;
        }

        public IRequestOptionsBuilder SetPort(ushort port)
        {
            _options.Port = port;

            return this;
        }

        public RequestOptions GetResult()
        {
            if (_options.Uri == null)
            {
                throw new OptionsBuildingException("Uri should not be null");
            }

            if (_options.Method == null)
            {
                throw new OptionsBuildingException("Method should not be null");
            }

            _options.Headers ??= new Dictionary<string, string>();

            if (!_options.Headers.ContainsKey("Host"))
            {
                AddHeader("Host", $"{_options.Uri.Host}");
            }

            var result = _options;
            _options = new RequestOptions();

            return result;
        }
    }
}

