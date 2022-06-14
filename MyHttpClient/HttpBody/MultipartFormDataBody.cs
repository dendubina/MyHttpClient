using System;
using System.Collections.Generic;
using MyHttpClientProject.Extensions;

namespace MyHttpClientProject.HttpBody
{
    public class MultipartFormDataBody : HttpBodyBase
    {
        public string Boundary { get; }
        internal Dictionary<string, HttpBodyBase> Contents = new();

        public MultipartFormDataBody()
        {
            MediaType = "multipart/form-data";
            Boundary = GetDefaultBoundary();
        }

        public void Add(HttpBodyBase body, string name)
        {
            if (name.ContainsNewLine() || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid name format", nameof(name));
            }

            Contents.Add(name, body);
        }

        private static string GetDefaultBoundary() => Guid.NewGuid().ToString();
    }
}
