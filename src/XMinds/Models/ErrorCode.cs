using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    public static class ErrorCode
    {
        public const int ServerError = 0;
        public const int ServerUnavailable = 1;
        public const int TooManyRequests = 2;
        public const int AuthError = 21;
        public const int JwtTokenExpired = 22;
        public const int RefreshTokenExpired = 28;
        public const int WrongData = 40;
        public const int DuplicatedError = 42;
        public const int ForbiddenError = 50;
        public const int NotFoundError = 60;
        public const int MethodNotAllowed = 70;
    }
}
