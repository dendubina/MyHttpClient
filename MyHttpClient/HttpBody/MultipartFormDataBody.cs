using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyHttpClientProject.Extensions;

namespace MyHttpClientProject.HttpBody
{
    public class MultipartFormDataBody : IHttpBody
    {
        public string Boundary { get; } = Guid.NewGuid().ToString();

        public IDictionary<string, IHttpBody> HttpBodyParts { get; }

        public string MediaType { get; }

        public MultipartFormDataBody()
        {
            MediaType = $"multipart/form-data;boundary=\"{Boundary}\"";
            HttpBodyParts = new Dictionary<string, IHttpBody>();
        }

        public void Add(IHttpBody body, string fieldName)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body),"Body must not be null");
            }

            if (fieldName.NullOrWhiteSpaceOrContainsNewLine())
            {
                throw new ArgumentException("Invalid name format", nameof(fieldName));
            }

            HttpBodyParts.Add(fieldName, body);
        }

        public byte[] GetContent()
        {
            if (HttpBodyParts.Count == 0)
            {
                throw new InvalidOperationException("The sequence has no elements");
            }

            var result = new List<byte>();
            var lastHttpBodyPart = HttpBodyParts.Last();

            foreach (var httpBodyPart in HttpBodyParts)
            {
                StringBuilder header = new();

                header.AppendLine($"--{Boundary}");
                header.AppendLine($"Content-Disposition: form-data; name=\"{httpBodyPart.Key}\"");

                if (httpBodyPart.Value.MediaType != null)
                {
                    header.AppendLine($"Content-Type: {httpBodyPart.Value.MediaType}");
                }

                header.AppendLine();

                result.AddRange(Encoding.UTF8.GetBytes(header.ToString()));

                result.AddRange(httpBodyPart.Value.GetContent());

                result.AddRange(httpBodyPart.Equals(lastHttpBodyPart)
                    ? Encoding.UTF8.GetBytes($"--{Boundary}--")
                    : Encoding.UTF8.GetBytes(Environment.NewLine));
            }

            return result.ToArray();
        }
    }
}
