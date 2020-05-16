using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private IServerApi ServerApi { get; }

        public ProxyController(IServerApi serverApi)
        {
            ServerApi = serverApi;
        }

        [HttpGet]
        public object ProxyGet(string id)
        {
            return ServerApi.TestGet(id);
        }
    }
}
