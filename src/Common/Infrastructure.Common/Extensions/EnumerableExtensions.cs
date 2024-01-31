
namespace Infrastructure.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class EnumerableExtensions
    {
        public static bool In<T>(this T value, params T[] types )
        {
            return types.Contains(value);
        }

        /// <summary>
        /// Формирует строковое представление коллекции.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string StringJoin(this IEnumerable<string> source, string separator)
        {
            var builder = new StringBuilder();
            foreach (String item in source)
            {
                if (builder.Length != 0)
                    builder.Append(separator);
                builder.Append(item);
            }
            return builder.ToString();
        }
        /// <summary>
        /// Формирует строковое представление коллекции.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string StringJoin<TSource>(this IEnumerable<TSource> source, string separator)
        {
            var builder = new StringBuilder();
            foreach (TSource item in source)
            {
                if (builder.Length != 0)
                    builder.Append(separator);
                builder.Append(item.ToString());
            }
            return builder.ToString();
        }
        /// <summary>
        /// Формирует строковое представление коллекции.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="formatter"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string StringJoin<TSource>(this IEnumerable<TSource> source, Func<TSource, string> formatter, string separator)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            var builder = new StringBuilder();
            foreach (TSource item in source)
            {
                if (builder.Length != 0)
                    builder.Append(separator);
                builder.Append(formatter(item));
            }
            return builder.ToString();
        }
    }
}
