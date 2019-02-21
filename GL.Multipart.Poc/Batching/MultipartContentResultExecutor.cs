using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;

namespace GL.Multipart.Poc.Batching
{
    public class MultipartContentResultExecutor : IActionResultExecutor<MultipartContentResult>
    {
        public async Task ExecuteAsync(ActionContext context, MultipartContentResult result)
        {
            await result.ExecuteResultAsync(context);
        }
    }
}
