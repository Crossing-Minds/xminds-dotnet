using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class RefreshTokenExpiredException : XMindsErrorException
    {
        internal RefreshTokenExpiredException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Unauthorized, apiError)
        {
        }
    }
}
