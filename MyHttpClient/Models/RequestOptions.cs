using System;
using System.Collections.Generic;
using System.Net.Http;
using MyHttpClientProject.HttpBody;

namespace MyHttpClientProject.Models
{
    public class RequestOptions
    {
        public HttpMethod Method { get; set; }
        public Uri Uri { get; set; }
        public IDictionary<string, string> Headers { get; set; } 
        public IHttpBody Body { get; set; }
        public ushort Port { get; set; }
    }
}
