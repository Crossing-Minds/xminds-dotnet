using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class RefreshTokenExpired : XMindsErrorException
    {
        internal RefreshTokenExpired(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Unauthorized, apiError)
        {
        }
    }
}
