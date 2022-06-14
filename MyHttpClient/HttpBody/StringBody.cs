
using System.Text;

namespace MyHttpClientProject.HttpBody
{
    public class StringBody : HttpBodyBase
    {
        private const string DefaultMediaType = "text/plain";

        public StringBody(string content) : this(content, DefaultEncoding)
        {
        }
        public StringBody(string content, Encoding encoding) : this(content, encoding, DefaultMediaType)
        {
        }
        public StringBody(string content, Encoding encoding, string contentType)
        {
            BufferedContent = encoding.GetBytes(content);
            MediaType = contentType + $"; charset={encoding.BodyName}";
        }
    }
}
