using System;

namespace MyHttpClientProject.Extensions
{
    internal static class StringExtensions
    {
        internal static bool ContainsNewLine(this string str) => str.AsSpan(0).IndexOfAny('\r', '\n') != -1;
    }
}
