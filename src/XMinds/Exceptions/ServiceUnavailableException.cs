using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The server is currently unavailable, please try again later. 
    /// We recommend employing an exponential backoff retry scheme.
    /// </summary>
    public class ServiceUnavailableException : XMindsErrorException
    {
        internal ServiceUnavailableException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.ServiceUnavailable, apiError)
        {
        }
    }
}
