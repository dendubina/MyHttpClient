using System;
using System.Linq;

namespace MyHttpClientProject.HttpBody
{
    public class ByteArrayBody : IHttpBody
    {
        public string MediaType { get; }
        private readonly byte[] _content;

        public ByteArrayBody(byte[] content) 
        {
            if (content is null || !content.Any())
            {
                throw new ArgumentException("Content must not be null or empty");
            }

            _content = content;
        }

        public ByteArrayBody(byte[] content, string mediaType) : this(content)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
            {
                throw new ArgumentException("Media type must not be null  empty");
            }
            
            MediaType = mediaType;
        }

        public byte[] GetContent() => _content;
    }
}
