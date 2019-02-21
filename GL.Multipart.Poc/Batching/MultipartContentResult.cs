using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace GL.Multipart.Poc.Batching
{
    public class MultipartContentResult : IActionResult
    {
        private readonly MultipartContent _content;

        public MultipartContentResult(MultipartContent content)
        {
            _content = content;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await _content.CopyToAsync(context.HttpContext.Response.Body);
        }
    }
}
