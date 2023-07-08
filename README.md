### Fluent, thread-safe HTTP client that uses only TCP connection

#### Usage examples:

```C#
static async Task Main(string[] args)
        {
            using var client = new MyHttpClient();

            // Get request example
            var response1 = await client.GetAsync("http://google.com");

            // Post request example
            var response2 = await client.PostAsync("http://google.com", new ByteArrayBody(new byte[] { 1, 2, 3 }, "media-type"));

            // Optional request example
            var options = new RequestOptionsBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri("http://google.com")
                .SetConnectionHeader(closeConnection: true)
                .AddOrChangeHeader("name", "value")
                .SetBody(new StringBody("content", Encoding.UTF8, "content-type"))
                .SetReceiveTimeout(100)
                .SetSendTimeout(100)
                .SetPort(123)
                .Build();

            var response3 = await client.SendAsync(options);

            //Response example
            Console.WriteLine(response1.StatusCode);

            foreach (var header in response1.Headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            Console.WriteLine(Encoding.UTF8.GetString(response1.Body));
        }
```
