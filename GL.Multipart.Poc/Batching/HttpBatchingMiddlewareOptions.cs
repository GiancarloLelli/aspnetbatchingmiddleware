using System.Net.Http;

namespace GL.Multipart.Poc.Batching
{
    public class HttpBatchingMiddlewareOptions
    {
        public HttpClient Client { get; set; }
    }
}
