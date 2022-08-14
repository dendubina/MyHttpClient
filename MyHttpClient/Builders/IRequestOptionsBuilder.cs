using System.Net.Http;
using MyHttpClientProject.HttpBody;
using MyHttpClientProject.Models;

namespace MyHttpClientProject.Builders
{
    public interface IRequestOptionsBuilder
    {
        IRequestOptionsBuilder SetMethod(HttpMethod method);
        IRequestOptionsBuilder SetUri(string uri);
        IRequestOptionsBuilder AddOrChangeHeader(string name, string value);
        IRequestOptionsBuilder SetBody(IHttpBody body);
        IRequestOptionsBuilder SetPort(ushort port);
        IRequestOptionsBuilder SetSendTimeout(int milliseconds);
        IRequestOptionsBuilder SetReceiveTimeout(int milliseconds);
        RequestOptions Build();
    }
}
