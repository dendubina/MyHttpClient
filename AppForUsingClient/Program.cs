using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppForUsingClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var line = "Connection: close    g";

            var pattern = @"(\w+):\s(\w+)";

            
            var result = new Regex(pattern).Match(line);

            Console.WriteLine(result.Success);

            foreach (var group in result.Groups)
            {
                Console.WriteLine(group);
            }




            Console.WriteLine("Hello world!");
            Console.ReadLine();
        }
    }
}
