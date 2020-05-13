using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpenFeign.net
{
    public class OpenFeign
    {
        public static T GenerateClient<T>()
        {
            return DispatchProxy.Create<T, HttpProxy>();
        }
    }
}
