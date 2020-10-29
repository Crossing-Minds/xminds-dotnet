using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XMinds.Models;

namespace XMinds.Exceptions
{
    public class NotFoundErrorException : XMindsErrorException
    {
        internal NotFoundErrorException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.NotFound, apiError)
        {
        }
    }
}
