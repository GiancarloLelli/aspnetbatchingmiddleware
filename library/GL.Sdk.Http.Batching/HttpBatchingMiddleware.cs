using GL.Sdk.Http.Batching.Configuration;
using GL.Sdk.Http.Batching.Extensions;
using GL.Sdk.Http.Batching.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GL.Sdk.Http.Batching
{
    public class HttpBatchingMiddleware
    {
        private const int BOUNDARY_LIMIT = 70;
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
            var isBatchedContentType = contentType.Contains("multipart/batch");

            if (!isBatchedContentType)
            {
                await _next.Invoke(context);
            }
            else
            {
                // Start a multipart container 
                var outerContent = new MultipartContent("batch");

                // Read the multipart request
                var mediaTypeHeaderValue = MediaTypeHeaderValue.Parse(contentType);
                var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeaderValue.Boundary);

                if (boundary.Length <= BOUNDARY_LIMIT)
                {
                    var reader = new MultipartReader(boundary.Value, context.Request.Body);
                    var section = await reader.ReadNextSectionAsync();

                    while (section != null)
                    {
                        try
                        {
                            using (var bodyreader = new StreamReader(section.Body))
                            {
                                var rawRequest = await bodyreader.ReadToEndAsync();
                                var request = rawRequest.ReadAsHttpRequestMessage();
                                var innerResp = await _options.Client.SendAsync(request, CancellationToken.None);
                                outerContent.Add(new HttpMessageContent(innerResp));
                            }
                        }
                        catch (Exception ex)
                        {
                            var exceptionReq = new HttpResponseMessage(HttpStatusCode.BadRequest);
                            exceptionReq.ReasonPhrase = ex.Message;
                            outerContent.Add(new HttpMessageContent(exceptionReq));
                        }

                        section = await reader.ReadNextSectionAsync();
                    }

                    var routeData = new RouteData();
                    var emptyActionDescriptor = new ActionDescriptor();
                    var actionContext = new ActionContext(context, routeData, emptyActionDescriptor);
                    var executor = context.RequestServices.GetService(typeof(MultipartContentResultExecutor)) as MultipartContentResultExecutor;
                    await executor.ExecuteAsync(actionContext, new MultipartContentResult(outerContent));

                    return;
                }
            }
        }
    }
}
