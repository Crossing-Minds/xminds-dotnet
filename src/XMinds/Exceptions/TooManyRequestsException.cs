using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The amount of requests exceeds the limit of your subscription.
    /// </summary>
    public class TooManyRequestsException : XMindsErrorException
    {
        internal const int TooManyRequestsHttpStatusCode = 429;

        internal TooManyRequestsException(ApiError apiError)
            : base((int)TooManyRequestsHttpStatusCode, apiError)
        {
        }
    }
}
