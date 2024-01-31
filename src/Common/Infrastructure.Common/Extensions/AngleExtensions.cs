
namespace Infrastructure.Common.Extensions
{
    using System;

    public static class AngleExtensions
    {
        /// <summary>
        /// Перевод градусов в радианы
        /// </summary>
        /// <param name="degreeAngle">Исходное значение в градусах</param>
        /// <returns>Возвращает значение в радианах</returns>
        public static double DegreeToRadian(this double degreeAngle)
        {
            return Math.PI * degreeAngle / 180d;
        }

        /// <summary>
        /// Перевод радиан в градусы
        /// </summary>
        /// <param name="radianAngle">Исходное значение в радинах</param>
        /// <returns>Возвращает значение в градусах</returns>
        public static double RadianToDegree(this double radianAngle)
        {
            return radianAngle * 180d / Math.PI;
        }
    }
}
