namespace MyHttpClientProject.Extensions
{
    internal static class StringExtensions
    {
        internal static bool ContainsNewLine(this string str) => str.IndexOfAny(new [] { '\r', '\n' } ) != -1;
    }
}
