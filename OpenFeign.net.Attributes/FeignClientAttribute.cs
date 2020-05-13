using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Attributes
{
    public sealed class FeignClientAttribute : Attribute
    {
        public string Name { get; set; }

        public string ContextId { get; set; }

        public string Url { get; set; }

        public bool Decode404 { get; set; } = false;

        public IFallback Fallback { get; set; }

        public string Path { get; set; }
    }
}
