namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class AboveLessConverter : JsonConverter<AboveLess>
    {
        public override void WriteJson(JsonWriter writer, AboveLess value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override AboveLess ReadJson(JsonReader reader, Type objectType, AboveLess existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return AboveLess.FromName(name);
        }
    }

    [JsonConverter(typeof(AboveLessConverter))]
    public class AboveLess : Enumeration
    {
        public static AboveLess Above = new AboveLess(1, "Above");
        public static AboveLess Less = new AboveLess(2, "Less");

        public AboveLess(int id, string name)
            : base(id, name)
        {
        }
        public static IEnumerable<AboveLess> List()
        {
            return new[]
            {
                Above,
                Less,
            };
        }

        public static AboveLess FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(AboveLess).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }

        public static AboveLess From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(AboveLess).Name}: {string.Join(",", List().Select(s => s.Id))}");
            }
            return state;
        }
    }
}
