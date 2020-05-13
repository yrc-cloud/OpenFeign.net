using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Api
{
    public class EchoObject
    {
        public string RequestUrl { get; set; }

        public string RequestMethod { get; set; }

        public Dictionary<string, string> QueryParameters { get; set; }

        public object BodyObject { get; set; }
    }
}
