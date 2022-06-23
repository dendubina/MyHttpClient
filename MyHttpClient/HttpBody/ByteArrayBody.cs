using System;

namespace MyHttpClientProject.HttpBody
{
    public class ByteArrayBody : IHttpBody
    {
        public string MediaType { get; }
        private readonly byte[] _content;

        public ByteArrayBody(byte[] content) : this(content, null)
        {

        }

        public ByteArrayBody(byte[] content, string mediaType)
        {
            if (content == null || content.Length == 0)
            {
                throw new ArgumentException("Content must not be null or empty");
            }

            if (mediaType == string.Empty)
            {
                throw new ArgumentException("Media type must not be empty");
            }

            _content = content;
            MediaType = mediaType;
        }

        public byte[] GetContent() => _content;
    }
}
