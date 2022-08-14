using System;
using MyHttpClientProject.Extensions;

namespace MyHttpClientProject.HttpBody
{
    public class ByteArrayBody : IHttpBody
    {
        private readonly byte[] _content;
        public string MediaType { get; }

        public ByteArrayBody(byte[] content) 
        {
            _content = content ?? throw new ArgumentException("Content must not be null");
        }

        public ByteArrayBody(byte[] content, string mediaType) : this(content)
        {
            if (string.IsNullOrWhiteSpace(mediaType) ||  mediaType.ContainsNewLine())
            {
                throw new ArgumentException("Invalid mediaType value");
            }
            
            MediaType = mediaType;
        }

        public byte[] GetContent() => _content;
    }
}
