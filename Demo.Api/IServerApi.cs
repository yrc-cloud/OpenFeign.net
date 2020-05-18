using System;
using OpenFeign.net.Attributes;

namespace Demo.Api
{
    [FeignClient(Name = "server", Url = "http://localhost:5005/")]
    public interface IServerApi
    {
        [HttpGet(Path = "api/test")]
        EchoObject TestGet([QueryParameter]string id);
    }
}
