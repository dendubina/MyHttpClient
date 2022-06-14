using System.Collections.Generic;
using System.Net;

namespace MyHttpClientProject.Models
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public IDictionary<string, string> ResponseHeaders { get; set; }

        public IEnumerable<byte> ResponseBody { get; set; }
    }
}
