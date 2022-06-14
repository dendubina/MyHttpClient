using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MyHttpClientProject.Builders;

namespace MyHttpClientProject.Extensions
{
    public static class RequestOptionsBuilderExtensions
    {
        public static IRequestOptionsBuilder SetAcceptHeader(this IRequestOptionsBuilder builder, string mediaType)
        {
            return SetAcceptHeader(builder, mediaType, 0);
        }

        public static IRequestOptionsBuilder SetAcceptHeader(this IRequestOptionsBuilder builder, string mediaType, double quality)
        {
            return SetAcceptHeader(builder, new Dictionary<string, double> { { mediaType, quality } });
        }

        public static IRequestOptionsBuilder SetAcceptHeader(this IRequestOptionsBuilder builder, IDictionary<string, double> mediaTypesWithQuality)
        {
            StringBuilder result = new();
            var last = mediaTypesWithQuality.Last();

            foreach (var item in mediaTypesWithQuality)
            {
                result.Append(item.Key);

                if (item.Value > 0)
                {
                    result.Append($";={item.Value.ToString("0.0", CultureInfo.InvariantCulture)}");
                }

                if (!item.Equals(last))
                {
                    result.Append(',');
                }
            }

            builder.AddHeader("Accept", result.ToString());

            return builder;
        }


        public static IRequestOptionsBuilder SetConnectionHeader(this IRequestOptionsBuilder builder, bool connectionClose)
        {
            builder.AddHeader("Connection", connectionClose ? "close" : "keep-alive");

            return builder;
        }

        //to be continued (about 15 headers)
    }
}
