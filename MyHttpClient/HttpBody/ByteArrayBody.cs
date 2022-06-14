
namespace MyHttpClientProject.HttpBody
{
    public class ByteArrayBody : HttpBodyBase
    {
        public ByteArrayBody(byte[] content) : this (content, null)
        {
            
        }
        public ByteArrayBody(byte[] content, string mediaType)
        {
            BufferedContent = content;

            if (mediaType != null)
            {
                MediaType = mediaType;
            }
        }
    }
}
