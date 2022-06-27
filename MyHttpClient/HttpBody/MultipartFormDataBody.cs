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

        public string Boundary { get; }

        public string MediaType { get; } 

        public MultipartFormDataBody()
        {
            Boundary = Guid.NewGuid().ToString();

            MediaType = $"multipart/form-data;boundary=\"{Boundary}\"";
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

            if (fileName != null && fileName.All(char.IsWhiteSpace) || fileName != null && fileName.ContainsNewLine())
            {
                throw new ArgumentException("Invalid fileName format", nameof(fileName));
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
                result.AddRange(Encoding.UTF8.GetBytes(GetHeadersStringForDataItem(httpBodyPart)));

                result.AddRange(Encoding.UTF8.GetBytes(Environment.NewLine));

                result.AddRange(httpBodyPart.Data.GetContent());

               // result.AddRange(Encoding.UTF8.GetBytes(Environment.NewLine));

                result.AddRange(httpBodyPart.Equals(lastHttpBodyPart)
                    ? Encoding.UTF8.GetBytes($"{Environment.NewLine}--{Boundary}--")
                    : Encoding.UTF8.GetBytes(Environment.NewLine));
            }

            return result.ToArray();
        }

        private string GetHeadersStringForDataItem(DataItem dataItem)
        {
            var result = new StringBuilder();

            result.AppendLine($"--{Boundary}");
            result.Append($"Content-Disposition: form-data; name=\"{dataItem.FieldName}\"");

            result.Append(dataItem.FileName != null
                ? $"; filename=\"{dataItem.FileName}\"{Environment.NewLine}"
                : Environment.NewLine);

            if (dataItem.Data.MediaType != null)
            {
                result.AppendLine($"Content-Type: {dataItem.Data.MediaType}");
            }

            return result.ToString();
        }
    }
}
