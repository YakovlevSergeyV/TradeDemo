namespace Infrastructure.Common.Extensions
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Усечение числа до указанного количества символов после запятой
    /// </summary>
    public static class DoubleExtensions
    {
        public static double TruncateKeepDecimalPlaces(this double value, int numDecimals)
        {
            var x = Math.Pow(10, numDecimals);
            return Math.Truncate(value * x) / x;
        }

        /// <summary>
        /// Определение количества символов после запятой
        /// </summary>
        /// <returns></returns>
        public static int GetNumberDecimals(this double value)
        {
            var str = Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture)
                .Split(CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator.ToCharArray());
            return str.Length == 2 ? str[1].Length : 0;
        }
    }
}
