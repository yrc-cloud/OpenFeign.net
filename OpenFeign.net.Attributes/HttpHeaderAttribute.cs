using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public class HttpHeaderAttribute : ParameterAttribute
    {
        public override ParameterType ParameterType { get; } = ParameterType.Header;
        public override string Name { get; set; }
    }
}
