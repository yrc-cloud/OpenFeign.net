using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public abstract class ParameterAttribute : Attribute
    {
        public abstract ParameterType ParameterType { get; }
        public abstract string Name { get; set; }
    }

    public enum ParameterType
    {
        Path = 1,
        Query = 2,
        Body = 3,
        Header = 4
    }
}
