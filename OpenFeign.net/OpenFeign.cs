using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpenFeign.net
{
    public class OpenFeign
    {
        public static T GenerateClient<T>() where T:class
        {
            return HttpProxy.Create<T>();
        }

        public static object GenerateClientByType(Type type)
        {
            var method = typeof(OpenFeign).GetMethod(nameof(GenerateClient));
            var generic = method.MakeGenericMethod(type);
            return generic.Invoke(null, null);
        }
    }
}
