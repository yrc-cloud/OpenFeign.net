using OpenFeign.net.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Retry
{
    public class DefaultRetry : IRetry
    {
        private const int MaxRetryCount = 3;
        public bool shouldRetry(RetryException retryException)
        {
            if (retryException.RetryCount >= MaxRetryCount) return false;
            retryException.RetryCount++;
            return true;
        }
    }
}
