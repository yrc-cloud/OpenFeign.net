using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class QueryParameterAttribute : ParameterAttribute
    {
        public override ParameterType ParameterType => ParameterType.Query;
        public override string Name { get; set; }
    }
}
