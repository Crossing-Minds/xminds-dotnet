using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The reported error codes.
    /// </summary>
    public static class ErrorCode
    {
        /// <summary>
        /// The server encountered an internal error. You may be able to retry your request, 
        /// but this usually indicates an error on the API side that require support.
        /// </summary>
        public const int ServerError = 0;

        /// <summary>
        /// The server is currently unavailable, please try again later. 
        /// We recommend employing an exponential backoff retry scheme.
        /// </summary>
        public const int ServerUnavailable = 1;

        /// <summary>
        /// The amount of requests exceeds the limit of your subscription.
        /// </summary>
        public const int TooManyRequests = 2;

        /// <summary>
        /// Cannot perform authentication.
        /// </summary>
        public const int AuthError = 21;

        /// <summary>
        /// The JWT token has expired.
        /// </summary>
        public const int JwtTokenExpired = 22;

        /// <summary>
        /// The refresh token has expired.
        /// </summary>
        public const int RefreshTokenExpired = 28;

        /// <summary>
        /// There is an error in the submitted data.
        /// </summary>
        public const int WrongData = 40;

        /// <summary>
        /// Some resource is duplicated.
        /// </summary>
        public const int DuplicatedError = 42;

        /// <summary>
        /// You do not have enough permissions to access this resource.
        /// </summary>
        public const int ForbiddenError = 50;

        /// <summary>
        /// Some resource does not exist.
        /// </summary>
        public const int NotFoundError = 60;

        /// <summary>
        /// The HTTP method is not allowed.
        /// </summary>
        public const int MethodNotAllowed = 70;
    }
}
