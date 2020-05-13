using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class PathParameterAttribute : ParameterAttribute
    {
        public override ParameterType ParameterType => ParameterType.Path;
        public override string Name { get; set; }
    }
}
