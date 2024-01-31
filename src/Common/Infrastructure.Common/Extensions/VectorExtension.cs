
namespace Infrastructure.Common.Extensions
{
    using System;

    public static class VectorExtension
    {
        /// <summary>
        /// Определяет угол наклона 
        /// </summary>
        /// <returns>Возвращает угол в градусах</returns>
        public static double AngleBetween(this double price1, double price2)
        {
            return AngleBetween(0, price1, X(price1, price2), price2);
        }

        /// <summary>
        /// Определяет угол наклона 
        /// </summary>
        /// <returns>Возвращает угол в градусах</returns>
        public static double AngleBetween(double x1, double y1, double x2, double y2)
        {
            var b = (y2 - y1);
            var a = (x2 - x1);

            return Math.Atan(b / a) * (180.0 / Math.PI);
        }

        private static double X(double price1, double price2)
        {
            var minValue = Math.Min(price1, price2);
            if (minValue == 0) return 0;

            var x = 10.0;

            if (minValue < 1.0)
            {
                x = 1.0;
                while (true)
                {
                    minValue *= 10;
                    if (minValue >= 1.0) break;
                    x *= 0.1;
                }
            }

            return (double)(decimal)x;
        }
    }
}
