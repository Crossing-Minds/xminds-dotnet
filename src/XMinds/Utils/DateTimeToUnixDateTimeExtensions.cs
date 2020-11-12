using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds.Utils
{
    static class DateTimeToUnixDateTimeExtensions
    {
        private static readonly DateTime EpochDateTime = new DateTime(1970, 1, 1);

        public static double ToUnixDateTime(this DateTime datetime)
        {
            return (datetime.Subtract(EpochDateTime)).TotalSeconds;
        }

        public static DateTime ToDateTimeFromUnixDateTime(this double datetime)
        {
            return EpochDateTime.AddSeconds(datetime);
        }
    }
}
