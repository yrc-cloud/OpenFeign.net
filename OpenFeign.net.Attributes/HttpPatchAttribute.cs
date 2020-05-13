using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class HttpPatchAttribute : HttpRequestMethodAttribute
    {
        public override HttpMethod Method => new HttpMethod("PATCH");
        public override string Path { get; set; }
    }
}
