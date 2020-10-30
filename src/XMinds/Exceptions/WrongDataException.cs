using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    public class WrongDataException : XMindsErrorException
    {
        internal WrongDataException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.BadRequest, apiError)
        {
        }
    }
}
