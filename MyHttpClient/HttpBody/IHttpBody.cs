
using System.Text;

namespace MyHttpClientProject.HttpBody
{
    public interface IHttpBody
    {
        string MediaType { get; }

        byte[] GetContent();
    }
}
