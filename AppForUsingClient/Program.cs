using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyHttpClientProject;
using MyHttpClientProject.Builders;
using MyHttpClientProject.Extensions;
using MyHttpClientProject.HttpBody;

namespace AppForUsingClient
{
    public class Program
    {

        static async Task Main(string[] args)
        {
            /*using var client = new MyHttpClient();
            var builder = new RequestOptionsBuilder();

            var request = builder
                .SetMethod(HttpMethod.Get)
                .SetUri("http://onreader.mdl.ru/MasteringConcurrencyInPython/content/figures/Fig0502.jpg")
                .Build();

            var response = await client.GetResponseAsync(request);

            Console.WriteLine($"{(int)response.StatusCode} {response.StatusCode}");

            foreach (var (name, value) in response.ResponseHeaders)
            {
                Console.WriteLine($"{name}: {value}");
            }

            if (response.ResponseBody != null)
            {
                Console.WriteLine();
                Console.WriteLine(Encoding.UTF8.GetString(response.ResponseBody.ToArray()));
            }*/

            var parameters = new Dictionary<string, double>
            {
                { "mediaType1", 0.5 },
                { "mediaType2", 1 },
                { "mediaType3", 0.8 }
            };

            var result = new RequestOptionsBuilder()
                .SetAcceptHeader(parameters);

            Console.WriteLine("GAME OVER");
            Console.ReadLine();
        }
    }
}
