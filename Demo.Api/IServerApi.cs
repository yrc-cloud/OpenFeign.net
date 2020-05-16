using System;
using OpenFeign.net.Attributes;

namespace Demo.Api
{
    [FeignClient(Name = "server", Url = "http://localhost:5000/")]
    public interface IServerApi
    {
        [HttpGet]
        EchoObject TestGet([QueryParameter]string id);
    }
}
