using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class MethodNotAllowedException : XMindsErrorException
    {
        internal MethodNotAllowedException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.MethodNotAllowed, apiError)
        {
        }
    }
}
