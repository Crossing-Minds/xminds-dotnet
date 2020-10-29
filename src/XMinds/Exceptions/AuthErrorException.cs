using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XMinds.Models;

namespace XMinds.Exceptions
{
    public class AuthErrorException : XMindsErrorException
    {
        internal AuthErrorException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Unauthorized, apiError)
        {
        }
    }
}
