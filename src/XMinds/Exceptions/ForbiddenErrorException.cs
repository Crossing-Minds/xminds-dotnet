using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// You do not have enough permissions to access this resource.
    /// </summary>
    public class ForbiddenErrorException : XMindsErrorException
    {
        internal ForbiddenErrorException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Forbidden, apiError)
        {
        }
    }
}
