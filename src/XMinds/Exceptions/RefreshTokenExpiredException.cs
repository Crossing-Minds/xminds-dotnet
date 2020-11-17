using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The refresh token has expired.
    /// </summary>
    public class RefreshTokenExpiredException : XMindsErrorException
    {
        internal RefreshTokenExpiredException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Unauthorized, apiError)
        {
        }
    }
}
