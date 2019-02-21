using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GL.Multipart.Poc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return $"Sent: {id}";
        }

        [HttpPost]
        public ActionResult<string> Get()
        {
            return new StreamReader(Request.Body).ReadToEnd();
        }
    }
}
