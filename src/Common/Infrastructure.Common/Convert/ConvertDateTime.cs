namespace Infrastructure.Common.Convert
{
    using System;
    using Infrastructure.Common.Ambient;

    public static class ConvertDateTime
    {
        public static DateTime UnixTimeStampMillisecondsToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddMilliseconds(unixTimeStamp);
        }

        public static DateTime UnixTimeStampSecondsToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return dtDateTime.AddSeconds(unixTimeStamp);
        }

        public static long CurrentDateToUnixTimeStampMilliseconds()
        {
            return DateToUnixTimeStampMilliseconds(TimeProvider.Current.UtcNow);
        }

        public static long DateToUnixTimeStampMilliseconds(DateTime date)
        {
            var unixTime = date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)unixTime.TotalMilliseconds;
        }

        public static long DateToUnixTimeStampSeconds(DateTime date)
        {
            var unixTime = date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)unixTime.TotalSeconds;
        }

        public static long ToUnixSeconds(this DateTime date)
        {
            var unixTime = date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)unixTime.TotalSeconds;
        }
    }
}
