using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The background task status.
    /// </summary>
    public static class BackgroundTaskStatus
    {
        public const string RequestSent = "REQUEST_SENT";
        public const string Running = "RUNNING";
        public const string Failed = "FAILED";
        public const string Completed = "COMPLETED";
    }
}
