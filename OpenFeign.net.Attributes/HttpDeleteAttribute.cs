using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class HttpDeleteAttribute : HttpRequestMethodAttribute
    {
        public override HttpMethod Method => HttpMethod.Delete;
        public override string Path { get; set; }
    }
}
