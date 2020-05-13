using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class HttpGetAttribute : HttpRequestMethodAttribute
    {
        public override HttpMethod Method => HttpMethod.Get;
        public override string Path { get; set; }
    }
}
