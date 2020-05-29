﻿using OpenFeign.net.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFeign.net.Retry
{
    public interface IRetry
    {
        bool shouldRetry(RetryException retryException);
    }
}
