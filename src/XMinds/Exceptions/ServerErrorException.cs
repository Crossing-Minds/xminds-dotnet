using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XMinds.Models;

namespace XMinds.Exceptions
{
    public class ServerErrorException : XMindsErrorException
    {
        internal ServerErrorException(int httpStatusCode,  ApiError apiError)
            : base(httpStatusCode, apiError)
        {
        }
    }
}
