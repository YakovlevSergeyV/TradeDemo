namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class OrderTypesConverter : JsonConverter<OrderTypes>
    {
        public override void WriteJson(JsonWriter writer, OrderTypes value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override OrderTypes ReadJson(JsonReader reader, Type objectType, OrderTypes existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return OrderTypes.FromName(name);
        }
    }

    [JsonConverter(typeof(OrderTypesConverter))]
    public class OrderTypes : Enumeration
    {
        public static OrderTypes Market = new OrderTypes(1, "Market"); // По рынку
        public static OrderTypes Limit = new OrderTypes(2, "Limit"); //Лимитный
        public static OrderTypes Stop = new OrderTypes(3, "Stop"); //Стоп
        public static OrderTypes StopLimit = new OrderTypes(4, "StopLimit"); //Стоп лимит

        protected OrderTypes() { }

        public OrderTypes(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderTypes> List()
        {
            return new[] { Market, Limit, Stop };
        }

        public static OrderTypes FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Возможные значения для {typeof(OrderTypes).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderTypes From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Возможные значения для {typeof(OrderTypes).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
