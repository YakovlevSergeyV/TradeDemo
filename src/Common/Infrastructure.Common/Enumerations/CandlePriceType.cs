namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class CandlePriceTypeConverter : JsonConverter<CandlePriceType>
    {
        public override void WriteJson(JsonWriter writer, CandlePriceType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override CandlePriceType ReadJson(JsonReader reader, Type objectType, CandlePriceType existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return CandlePriceType.FromName(name);
        }
    }

    [JsonConverter(typeof(CandlePriceTypeConverter))]
    public class CandlePriceType : Enumeration
    {
        public static CandlePriceType Open = new CandlePriceType(1, "Open");
        public static CandlePriceType Close = new CandlePriceType(2, "Close");
        public static CandlePriceType High = new CandlePriceType(3, "High");
        public static CandlePriceType Low = new CandlePriceType(4, "Low");
        public static CandlePriceType AverageHighLow = new CandlePriceType(5, "AverageHighLow");


        public CandlePriceType(int id, string name)
            : base(id, name)
        {

        }

        public static IEnumerable<CandlePriceType> List()
        {
            return new[]
            {
                Open,
                Close,
                High,
                Low,
                AverageHighLow
            };
        }

        public static CandlePriceType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(CandlePriceType).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static CandlePriceType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(CandlePriceType).Name}: {string.Join(",", List().Select(s => s.Id))}");
            }

            return state;
        }
    }
}
