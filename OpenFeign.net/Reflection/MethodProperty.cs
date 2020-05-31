using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenFeign.net.Reflection
{
    public class MethodProperty
    {
        public string Path { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public List<ParameterInfo> Parameters { get; set; }
    }
}
