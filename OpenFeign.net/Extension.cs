using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpenFeign.net
{
    public static class Extension
    {
        public static void AddFeignClient(this IServiceCollection service, Assembly assembly)
        {
            //TODO:user reflection to create all api client
            throw new NotImplementedException();
        }
    }
}
