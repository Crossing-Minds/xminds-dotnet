using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XMinds.Models;

namespace XMinds.Exceptions
{
    public class RefreshTokenExpired : XMindsErrorException
    {
        internal RefreshTokenExpired(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.Unauthorized, apiError)
        {
        }
    }
}
