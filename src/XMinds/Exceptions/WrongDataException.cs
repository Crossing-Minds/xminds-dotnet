using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XMinds.Models;

namespace XMinds.Exceptions
{
    public class WrongDataException : XMindsErrorException
    {
        internal WrongDataException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.BadRequest, apiError)
        {
        }
    }
}
