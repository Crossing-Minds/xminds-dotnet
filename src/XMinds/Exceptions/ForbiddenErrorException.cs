using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class ForbiddenErrorException : XMindsErrorException
    {
        internal ForbiddenErrorException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Forbidden, apiError)
        {
        }
    }
}
