using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class HttpPutAttribute: HttpRequestMethodAttribute
    {
        public override HttpMethod Method => HttpMethod.Put;
        public override string Path { get; set; }
    }
}
