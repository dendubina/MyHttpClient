using System;
using System.Collections.Generic;
using System.Net.Http;
using MyHttpClientProject.HttpBody;

namespace MyHttpClientProject.Models
{
    public class RequestOptions
    {
        public HttpMethod Method { get; internal set; }
        public Uri Uri { get; internal set; }
        public IDictionary<string, string> Headers { get; internal set; } 
        public IHttpBody Body { get; internal set; }
        public ushort Port { get; internal set; } = 80;

        internal RequestOptions()
        {
        }
    }
}
