using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class ServerErrorException : XMindsErrorException
    {
        internal ServerErrorException(int httpStatusCode,  ApiError apiError)
            : base(httpStatusCode, apiError)
        {
        }
    }
}
