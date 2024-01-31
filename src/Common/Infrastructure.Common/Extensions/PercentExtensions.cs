namespace Infrastructure.Common.Extensions
{
    using System;

    public static class PercentExtensions
    {
        /// <summary>
        /// Процентная разница между числами
        /// </summary>
        /// <param name="value1">Первое значение</param>
        /// <param name="value2">Второе значение</param>
        /// <returns>Возвращает разницу в процентах между числами</returns>
        public static double PercentageDifference(this double value1, double value2)
        {
            return Math.Abs((value1 - value2)/ ((value1 + value2)/2))*100;
        }
    }
}
