using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XMinds.Models;

namespace XMinds.Exceptions
{
    public class ForbiddenErrorException : XMindsErrorException
    {
        internal ForbiddenErrorException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Forbidden, apiError)
        {
        }
    }
}
