namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class YesNoConverter : JsonConverter<YesNo>
    {
        public override void WriteJson(JsonWriter writer, YesNo value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override YesNo ReadJson(JsonReader reader, Type objectType, YesNo existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return YesNo.FromName(name);
        }
    }

    [JsonConverter(typeof(YesNoConverter))]
    public class YesNo : Enumeration
    {
        public static YesNo Yes = new YesNo(1, "Yes");
        public static YesNo No = new YesNo(2, "No");

        public YesNo(int id, string name)
            : base(id, name)
        {
        }
        public static IEnumerable<YesNo> List()
        {
            return new[]
            {
                Yes,
                No,
            };
        }

        public static YesNo FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(YesNo).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }

        public static YesNo From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(YesNo).Name}: {string.Join(",", List().Select(s => s.Id))}");
            }
            return state;
        }
    }
}
