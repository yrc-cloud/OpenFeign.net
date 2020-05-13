using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class HeaderParameterAttribute : ParameterAttribute
    {
        public override ParameterType ParameterType => ParameterType.Header;
        public override string Name { get; set; }
    }
}
