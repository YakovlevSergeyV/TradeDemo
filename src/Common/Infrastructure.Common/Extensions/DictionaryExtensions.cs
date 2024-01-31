
namespace Infrastructure.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DictionaryExtensions
    {
        public static void AddRangeOverride<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            IDictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => dic[x.Key] = x.Value);
        }

        public static void AddRangeNewOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            IDictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x =>
            {
                if (!dic.ContainsKey(x.Key)) dic.Add(x.Key, x.Value);
            });
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            IDictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => dic.Add(x.Key, x.Value));
        }

        public static bool ContainsKeys<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<TKey> keys)
        {
            bool result = false;
            keys.ForEachOrBreak((x) =>
            {
                result = dic.ContainsKey(x);
                return result;
            });
            return result;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void ForEachOrBreak<T>(this IEnumerable<T> source, Func<T, bool> func)
        {
            foreach (var item in source)
            {
                bool result = func(item);
                if (result) break;
            }
        }


        public static void AddRange<K, T, TI>(this IDictionary<K, T> target, IEnumerable<TI> source, Func<TI, K> key,
            Func<TI, T> selector, bool set = true)
        {
            source.ToList().ForEach(i =>
            {
                var dKey = key(i);
                var dValue = selector(i);
                if (set)
                {
                    target[dKey] = dValue;
                }
                else
                {
                    target.Add(key(i), selector(i));
                }
            });

        }
    }
}
