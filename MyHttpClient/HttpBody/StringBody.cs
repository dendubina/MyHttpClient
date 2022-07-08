using System;
using System.Text;
using MyHttpClientProject.Extensions;

namespace MyHttpClientProject.HttpBody
{
    public class StringBody : IHttpBody
    {
        private const string DefaultMediaType = "text/plain";
        private readonly string _content;
        private readonly Encoding _encoding;

        public string MediaType { get; }

        public StringBody(string content) : this(content, Encoding.UTF8)
        {

        }

        public StringBody(string content, Encoding encoding) : this(content, encoding, DefaultMediaType)
        {

        }

        public StringBody(string content, Encoding encoding, string contentType)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content should not be null or empty", nameof(content));
            }

            if (string.IsNullOrWhiteSpace(contentType) || contentType.ContainsNewLine())
            {
                throw new ArgumentException("Content type should not be null or empty", nameof(contentType));
            }

            _content = content;
            _encoding = encoding;
            MediaType = $"{contentType}; charset={encoding.BodyName}";
        }

        public byte[] GetContent() => _encoding.GetBytes(_content);
    }
}
