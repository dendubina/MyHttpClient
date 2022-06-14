using System;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyHttpClientProject;
using MyHttpClientProject.Builders;
using MyHttpClientProject.Extensions;

namespace AppForUsingClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new MyHttpClient();
            var builder = new RequestOptionsBuilder();

            var request = builder
                .SetMethod(HttpMethod.Get)
                .SetUri("http://google.com")
                .GetResult();

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
            }

            Console.WriteLine("GAME OVER");
            Console.ReadLine();
        }
    }
}
