using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds.Utils
{
    static class DateTimeToUnixDateTimeExtensions
    {
        private static readonly DateTime EpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static double ToUnixDateTime(this DateTime datetime)
        {
            return (datetime.ToUniversalTime().Subtract(EpochDateTime)).TotalSeconds;
        }

        public static DateTime ToDateTimeFromUnixDateTime(this double datetime)
        {
            return EpochDateTime.AddSeconds(datetime);
        }
    }
}
