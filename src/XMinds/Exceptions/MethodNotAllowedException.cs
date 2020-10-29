using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XMinds.Models;

namespace XMinds.Exceptions
{
    public class MethodNotAllowedException : XMindsErrorException
    {
        internal MethodNotAllowedException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.MethodNotAllowed, apiError)
        {
        }
    }
}
