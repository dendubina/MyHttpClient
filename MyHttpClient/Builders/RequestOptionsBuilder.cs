using System;
using System.Collections.Generic;
using System.Net.Http;
using MyHttpClientProject.Exceptions;
using MyHttpClientProject.Extensions;
using MyHttpClientProject.HttpBody;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Builders
{
    public class RequestOptionsBuilder : IRequestOptionsBuilder
    {
        private RequestOptions _options = new();

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
            if (string.IsNullOrWhiteSpace(name) || name.ContainsNewLine())
            {
                throw new ArgumentException("Invalid header name format", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(value) || value.ContainsNewLine())
            {
                throw new ArgumentException("Invalid header value format", nameof(value));
            }

            _options.Headers ??= new Dictionary<string, string>();

            _options.Headers.Add(name, value);

            return this;
        }

        public IRequestOptionsBuilder SetBody(IHttpBody body)
        {
            _options.Body = body;

            return this;
        }

        public IRequestOptionsBuilder SetPort(ushort port)
        {
            _options.Port = port;

            return this;
        }

        public IRequestOptionsBuilder SetSendTimeout(int milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentException("Timeout must be > 0");
            }

            _options.SendTimeout = milliseconds;

            return this;
        }

        public IRequestOptionsBuilder SetReadTimeout(int milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentException("Timeout must be > 0");
            }

            _options.ReadTimeout = milliseconds;

            return this;
        }

        public RequestOptions Build()
        {
            ValidateRequiredValues();

            _options.Headers ??= new Dictionary<string, string>();

            if (_options.Port == 0)
            {
                SetPort(80);
            }

            AddRepresentationHeaders();

            AddHost();

            var result = _options;
            _options = new RequestOptions();

            return result;
        }

        private void ValidateRequiredValues()
        {
            if (_options.Uri == null)
            {
                throw new OptionsBuildingException("Uri should not be null");
            }

            if (_options.Method == null)
            {
                throw new OptionsBuildingException("Method should not be null");
            }
        }

        private void AddRepresentationHeaders()
        {
            if (_options.Body == null)
            {
                return;
            }

            if (_options.Body.MediaType != null && !_options.Headers.ContainsKey("Content-Type"))
            {
                AddHeader("Content-Type", _options.Body.MediaType);
            }

            if (!_options.Headers.ContainsKey("Content-Length"))
            {
                AddHeader("Content-Length", _options.Body.GetContent().Length.ToString());
            }
        }

        private void AddHost()
        {
            if (!_options.Headers.ContainsKey("Host"))
            {
                _options.Headers.Add("Host", _options.Uri.Host);
            }
        }
    }
}

