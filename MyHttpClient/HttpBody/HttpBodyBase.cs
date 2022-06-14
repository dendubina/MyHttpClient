using System.Text;

namespace MyHttpClientProject.HttpBody
{
    public abstract class HttpBodyBase
    {
        protected static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public string MediaType { get; protected set; }
        public byte[] BufferedContent { get; protected set; }
    }
}
