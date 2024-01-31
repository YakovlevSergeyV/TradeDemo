
namespace Infrastructure.Common.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static string UpToSecondsToString(this DateTime value)
        {
            return value.ToString("dd.MM.yyyy HH:mm:ss");
        }
    }
}
