using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// Some resource is duplicated.
    /// </summary>
    public class DuplicatedErrorException : XMindsErrorException
    {
        internal DuplicatedErrorException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.BadRequest, apiError)
        {
        }
    }
}
