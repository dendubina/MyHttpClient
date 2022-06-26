using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyHttpClientProject.Extensions;

namespace MyHttpClientProject.HttpBody
{
    public class MultipartFormDataBody : IHttpBody
    {
        private struct DataItem
        {
            public IHttpBody Data { get; init; }
            public string FieldName { get; init; }
            public string FileName { get; init; }
        }

        private readonly List<DataItem> _dataItems = new();
        private readonly string _boundary = Guid.NewGuid().ToString();
        public string MediaType { get; }

        public MultipartFormDataBody()
        {
            MediaType = $"multipart/form-data;boundary=\"{_boundary}\"";
        }

        public void Add(IHttpBody body, string fieldName, string fileName = null)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body),"Body must not be null");
            }

            if (string.IsNullOrWhiteSpace(fieldName) || fieldName.ContainsNewLine())
            {
                throw new ArgumentException("Invalid name format", nameof(fieldName));
            }

            if (fileName != null && fileName.All(char.IsWhiteSpace) || fileName.ContainsNewLine())
            {
                throw new ArgumentException("Invalid fileName format");
            }

            _dataItems.Add(new DataItem
            {
                Data = body, 
                FieldName = fieldName,
                FileName = fileName,
            });
        }

        public byte[] GetContent()
        {
            if (!_dataItems.Any())
            {
                throw new InvalidOperationException("The sequence has no elements");
            }

            var result = new List<byte>();
            var lastHttpBodyPart = _dataItems.Last();

            foreach (var httpBodyPart in _dataItems)
            {
                result.AddRange(GetHeaderBytesForDataItem(httpBodyPart));

                result.AddRange(Encoding.UTF8.GetBytes(Environment.NewLine));

                result.AddRange(httpBodyPart.Data.GetContent());

                result.AddRange(Encoding.UTF8.GetBytes(Environment.NewLine));

                result.AddRange(httpBodyPart.Equals(lastHttpBodyPart)
                    ? Encoding.UTF8.GetBytes($"--{_boundary}--")
                    : Encoding.UTF8.GetBytes(Environment.NewLine));
            }

            return result.ToArray();
        }

        private byte[] GetHeaderBytesForDataItem(DataItem dataItem)
        {
            var result = new List<byte>();

            result.AddRange(Encoding.UTF8.GetBytes($"--{_boundary}{Environment.NewLine}"));
            result.AddRange(Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"{dataItem.FieldName}\""));

            result.AddRange(dataItem.FileName != null
                ? Encoding.UTF8.GetBytes($"; filename=\"{dataItem.FileName}\"{Environment.NewLine}")
                : Encoding.UTF8.GetBytes(Environment.NewLine));

            if (dataItem.Data.MediaType != null)
            {
                result.AddRange(Encoding.UTF8.GetBytes($"Content-Type: {dataItem.Data.MediaType}{Environment.NewLine}"));
            }

            return result.ToArray();
        }
    }
}
