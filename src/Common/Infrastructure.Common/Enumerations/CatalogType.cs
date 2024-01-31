
namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class CatalogTypeConverter : JsonConverter<CatalogType>
    {
        public override void WriteJson(JsonWriter writer, CatalogType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override CatalogType ReadJson(JsonReader reader, Type objectType, CatalogType existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return CatalogType.FromName(name);
        }
    }

    [JsonConverter(typeof(CatalogTypeConverter))]
    public class CatalogType : Enumeration
    {
        public static CatalogType Catalog = new CatalogType(1, "Catalog");
        public static CatalogType Strategy = new CatalogType(2, "Strategy");

        public CatalogType(int id, string name)
            : base(id, name)
        {
        }
        public static IEnumerable<CatalogType> List()
        {
            return new[]
            {
                Catalog,
                Strategy,
            };
        }

        public static CatalogType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(CatalogType).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }

        public static CatalogType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(CatalogType).Name}: {string.Join(",", List().Select(s => s.Id))}");
            }
            return state;
        }
    }
}
