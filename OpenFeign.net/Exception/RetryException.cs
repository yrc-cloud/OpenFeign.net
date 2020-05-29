using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenFeign.net.Exception
{
    public class RetryException : FeignException
    {
        public RetryException(HttpRequestMessage request)
        {
            Reqeust = request;
            RetryCount = 1;
        }
        public int RetryCount { get; set; }
        public HttpRequestMessage Reqeust { get; }
    }
}
