using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The server encountered an internal error. You may be able to retry your request, 
    /// but this usually indicates an error on the API side that require support.
    /// </summary>
    public class ServerErrorException : XMindsErrorException
    {
        internal ServerErrorException(int httpStatusCode,  ApiError apiError)
            : base(httpStatusCode, apiError)
        {
        }
    }
}
