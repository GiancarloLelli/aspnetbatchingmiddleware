using GL.Sdk.Http.Batching.Configuration;
using GL.Sdk.Http.Batching.Extensions;
using GL.Sdk.Http.Batching.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GL.Sdk.Http.Batching
{
    public class HttpBatchingMiddleware
    {
        private const int BOUNDARY_LIMIT = 70;
        private const int BATCH_SIZE = 100;
        private const string CONTENT_TYPE = "multipart/batch";
        private const string SUBTYPE = "batch";

        private readonly RequestDelegate _next;
        private readonly HttpBatchingMiddlewareOptions _options;

        public HttpBatchingMiddleware(RequestDelegate next, IOptions<HttpBatchingMiddlewareOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var contentType = context.Request.ContentType ?? string.Empty;
            var isBatchedContentType = contentType.Contains(CONTENT_TYPE);

            if (!isBatchedContentType)
            {
                await _next.Invoke(context);
            }
            else
            {
                // Start a multipart container 
                var outerContent = new MultipartContent(SUBTYPE);

                // Read the multipart request
                var mediaTypeHeaderValue = MediaTypeHeaderValue.Parse(contentType);
                var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeaderValue.Boundary);

                if (boundary.Length <= BOUNDARY_LIMIT)
                {
                    var reader = new MultipartReader(boundary.Value, context.Request.Body);
                    var section = await reader.ReadNextSectionAsync();
                    var requestMessageCollection = new List<HttpRequestMessage>();

                    // Section to HTTP Request conversion
                    while (section != null)
                    {
                        using (var bodyreader = new StreamReader(section.Body))
                        {
                            var rawRequest = await bodyreader.ReadToEndAsync();
                            var request = rawRequest.ReadAsHttpRequestMessage();
                            requestMessageCollection.Add(request);
                        }

                        section = await reader.ReadNextSectionAsync();
                    }

                    // Parallel execution
                    var tasks = new List<Task<HttpResponseMessage>>();
                    requestMessageCollection.ForEach(x => tasks.Add(_options.Client.SendAsync(x, CancellationToken.None)));
                    (await Task.WhenAll(tasks)).ToList().ForEach(y => outerContent.Add(new HttpMessageContent(y)));

                    // Return the multipart response to caller
                    await context.ServeMultipartContent(new MultipartContentResult(outerContent));
                    return;
                }
            }
        }
    }
}
