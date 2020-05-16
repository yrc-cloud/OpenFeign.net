using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public EchoObject Get(string id)
        {
            return new EchoObject()
            {
                RequestMethod = "Get",
                RequestUrl = Request.GetDisplayUrl(),
                QueryParameters = new Dictionary<string, string>
                {
                    {"id", id }
                }
            };
        }
    }
}