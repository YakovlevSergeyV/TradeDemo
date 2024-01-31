
namespace Infrastructure.Common.Extensions
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class StringExtension
    {
        /// <summary>
        /// Проверка строки на соответствие JSON
        /// </summary>
        /// <param name="stringValue">Проверяемая строка</param>
        /// <returns></returns>
        public static bool IsValidJson(this string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                (value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }

        public static bool IsNullOrEmpty(this string stringValue)
        {
            return string.IsNullOrEmpty(stringValue);
        }

        public static string GetKey(this string exchange, string currencyPair)
        {
            return $"{exchange.ToUpper()}-{currencyPair.ToUpper()}";
        }
    }
}
