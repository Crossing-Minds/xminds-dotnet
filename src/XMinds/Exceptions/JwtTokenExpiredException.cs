using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class JwtTokenExpiredException : XMindsErrorException
    {
        internal JwtTokenExpiredException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Unauthorized, apiError)
        {
        }
    }
}
