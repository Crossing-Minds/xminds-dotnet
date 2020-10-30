using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class NotFoundErrorException : XMindsErrorException
    {
        internal NotFoundErrorException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.NotFound, apiError)
        {
        }
    }
}
