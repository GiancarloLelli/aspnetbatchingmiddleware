using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace GL.Multipart.Poc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return $"Sent: {id}";
        }

        [HttpPost]
        public ActionResult<string> Post()
        {
            return new StreamReader(Request.Body).ReadToEnd();
        }

        [HttpPut]
        public ActionResult<string> Put()
        {
            return new StreamReader(Request.Body).ReadToEnd();
        }

        [HttpPatch]
        public ActionResult<string> Patch()
        {
            return new StreamReader(Request.Body).ReadToEnd();
        }
    }
}
