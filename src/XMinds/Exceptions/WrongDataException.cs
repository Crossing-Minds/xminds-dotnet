using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// There is an error in the submitted data.
    /// </summary>
    public class WrongDataException : XMindsErrorException
    {
        internal WrongDataException(ApiError apiError)
            : base((int) System.Net.HttpStatusCode.BadRequest, apiError)
        {
        }
    }
}
