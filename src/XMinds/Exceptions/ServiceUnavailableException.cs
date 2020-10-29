using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XMinds.Models;

namespace XMinds.Exceptions
{
    public class ServiceUnavailableException : XMindsErrorException
    {
        internal ServiceUnavailableException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.ServiceUnavailable, apiError)
        {
        }
    }
}
