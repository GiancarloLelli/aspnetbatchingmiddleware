using System;
using System.Net.Http;
using System.Threading;

namespace GL.Multipart.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var reset = new ManualResetEvent(false);
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
            post.Content = new StringContent("Hello World Post");
            post.Headers.TryAddWithoutValidation("Content-Type", "text/plain");
            batchContent.Add(new HttpMessageContent(post));

            var put = new HttpRequestMessage(HttpMethod.Put, new Uri("http://localhost:54942/api/values/"));
            put.Content = new StringContent("Hello World Put");
            put.Headers.TryAddWithoutValidation("Content-Type", "text/plain");
            batchContent.Add(new HttpMessageContent(put));

            var patch = new HttpRequestMessage(HttpMethod.Patch, new Uri("http://localhost:54942/api/values/"));
            patch.Content = new StringContent("Hello World Patch");
            patch.Headers.TryAddWithoutValidation("Content-Type", "text/plain");
            batchContent.Add(new HttpMessageContent(patch));

            var result = client.SendAsync(batchRequest).GetAwaiter().GetResult();
            var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Console.WriteLine(result.ToString());
            Console.WriteLine(content.ToString());

            reset.WaitOne();
        }
    }
}
