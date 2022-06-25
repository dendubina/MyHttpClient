using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MyHttpClientProject.Builders;

namespace MyHttpClientProject.Extensions
{
    public static class RequestOptionsBuilderExtensions
    {
        public static IRequestOptionsBuilder SetAcceptHeader(this IRequestOptionsBuilder builder, string mediaType) => SetAcceptHeader(builder, mediaType, qFactor: 1);

        public static IRequestOptionsBuilder SetAcceptHeader(this IRequestOptionsBuilder builder, string mediaType, double qFactor)
        {
            return SetAcceptHeader(builder, new Dictionary<string, double> { { mediaType, qFactor } });
        }

        public static IRequestOptionsBuilder SetAcceptHeader(this IRequestOptionsBuilder builder, IDictionary<string, double> mediaTypesWithQFactor)
        {
            if (mediaTypesWithQFactor.Values.Any(x => x is > 1 or < 0))
            {
                throw new ArgumentException("Invalid q-factor value");
            }

            StringBuilder result = new();

            var sortedValues = mediaTypesWithQFactor
                .OrderByDescending(x => x.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            var last = sortedValues.Last();

            foreach (var item in sortedValues)
            {
                result.Append(item.Key);

                if (item.Value is > 0 and < 1)
                {
                    result.Append($";q={item.Value.ToString("0.0", CultureInfo.InvariantCulture)}");
                }

                if (!item.Equals(last))
                {
                    result.Append(", ");
                }
            }

            builder.AddHeader("Accept", result.ToString());

            return builder;
        }


        public static IRequestOptionsBuilder SetConnectionHeader(this IRequestOptionsBuilder builder, bool connectionClose) =>
            builder.AddHeader("Connection", connectionClose ? "close" : "keep-alive");


        //to be continued (about 15 headers)
    }
}
