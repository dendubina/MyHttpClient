using System;

namespace MyHttpClientProject.Extensions
{
    internal static class StringExtensions
    {
        internal static bool NullOrWhiteSpaceOrContainsNewLine(this string str) => str.IndexOfAny(new [] { '\r', '\n' } ) != -1 && !string.IsNullOrWhiteSpace(str);
    }
}
