using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class HttpPostAttribute : HttpRequestMethodAttribute
    {
        public override HttpMethod Method => HttpMethod.Post;
        public override string Path { get; set; }
    }
}
