using System;
using System.Collections.Generic;
using System.Text;
using OpenFeign.net.Retry;

namespace OpenFeign.net
{
    public class OpenFeignConfig
    {
        public IRetry Retry { get; set; }
    }
}
