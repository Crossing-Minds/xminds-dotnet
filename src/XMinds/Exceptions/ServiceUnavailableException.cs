using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class ServiceUnavailableException : XMindsErrorException
    {
        internal ServiceUnavailableException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.ServiceUnavailable, apiError)
        {
        }
    }
}
