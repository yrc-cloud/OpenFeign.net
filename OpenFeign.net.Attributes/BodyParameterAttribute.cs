using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class BodyParameterAttribute : ParameterAttribute
    {
        public override ParameterType ParameterType => ParameterType.Body;
        public override string Name { get; set; }
    }
}
