using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyHttpClientProject;
using MyHttpClientProject.Extensions;

namespace AppForUsingClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MyHttpClient();

            var response = await client.GetAsync("http://rasfokus.ru/images/photos/medium/5490c3cb296af723d10bf3812079e145.jpg");

            Console.WriteLine(response.StatusCode);

            client.Dispose();
            Console.WriteLine("Hello world!");
            Console.ReadLine();
        }
    }
}
