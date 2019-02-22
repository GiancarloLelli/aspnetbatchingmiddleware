using GL.Sdk.Http.Batching.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace GL.Sdk.Http.Batching.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task ServeMultipartContent(this HttpContext context, MultipartContentResult contentResult)
        {
            var routeData = new RouteData();
            var emptyActionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(context, routeData, emptyActionDescriptor);
            var executor = context.RequestServices.GetService(typeof(MultipartContentResultExecutor)) as MultipartContentResultExecutor;
            await executor.ExecuteAsync(actionContext, contentResult);
        }
    }
}
