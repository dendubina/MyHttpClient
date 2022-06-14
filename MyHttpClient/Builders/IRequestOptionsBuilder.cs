using System.Net.Http;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Builders
{
    public interface IRequestOptionsBuilder
    {
        IRequestOptionsBuilder SetMethod(HttpMethod method);
        IRequestOptionsBuilder SetUri(string uri);
        IRequestOptionsBuilder AddHeader(string name, string value);
        IRequestOptionsBuilder AddBody(HttpBody.HttpBodyBase body);
        IRequestOptionsBuilder SetPort(ushort port);
        RequestOptions GetResult();
    }
}
