using System;
using System.Net.Http;

namespace GL.Multipart.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();

            var batchRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "http://localhost:54942/batch"
            );

            var batchContent = new MultipartContent("batch");
            batchRequest.Content = batchContent;

            batchContent.Add(
                new HttpMessageContent(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        "http://localhost:54942/api/values/5"
                    )
                )
            );

            var post = new HttpRequestMessage(HttpMethod.Post, new Uri("http://localhost:54942/api/values/"));
            post.Content = new StringContent("Hello World");
            post.Headers.TryAddWithoutValidation("Content-Type", "text/plain");
            batchContent.Add(new HttpMessageContent(post));

            var result = client.SendAsync(batchRequest).GetAwaiter().GetResult();
            var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Console.WriteLine(result.ToString());
        }
    }
}
