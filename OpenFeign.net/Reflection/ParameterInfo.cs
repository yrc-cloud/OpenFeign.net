using OpenFeign.net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeign.net.Reflection
{
    public class ParameterInfo
    {
        public string Name { get; }
        public ParameterType Type { get; }
        public bool IsSimple { get; }

        private static readonly Type[] SimpleTypes = new Type[] {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        };

        public ParameterInfo(string name, ParameterType type, bool isSimple)
        {
            Name = name;
            Type = type;
            IsSimple = isSimple;
        }

        public static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive || SimpleTypes.Contains(type) ||
                type.IsEnum ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }
    }
}
