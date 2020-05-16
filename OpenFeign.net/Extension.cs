using Microsoft.Extensions.DependencyInjection;
using OpenFeign.net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenFeign.net
{
    public static class Extension
    {
        public static void AddFeignClient(this IServiceCollection service, Assembly assembly)
        {
            var interfaces = assembly.DefinedTypes.Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(FeignClientAttribute)) && t.IsInterface);
            foreach(var @interface in interfaces)
            {
                service.AddSingleton(@interface.GetType(), OpenFeign.GenerateClientByType(@interface.GetType()));
            }
            foreach (var refAssembly in assembly.GetReferencedAssemblies())
            {
                var ass = Assembly.Load(refAssembly);
                interfaces = ass.DefinedTypes.Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(FeignClientAttribute)) && t.IsInterface);

                foreach (var @interface in interfaces)
                {
                    service.AddSingleton(@interface, OpenFeign.GenerateClientByType(@interface));
                }
            }
        }
    }
}
