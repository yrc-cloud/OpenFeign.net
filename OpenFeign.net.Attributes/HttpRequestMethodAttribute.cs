using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public abstract class HttpRequestMethodAttribute : Attribute
    {
        public abstract HttpMethod Method { get; }
        public abstract string Path { get; set; }
    }
}
