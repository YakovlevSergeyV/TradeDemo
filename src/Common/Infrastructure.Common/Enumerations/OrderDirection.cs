namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class OrderDirectionConverter : JsonConverter<OrderDirection>
    {
        public override void WriteJson(JsonWriter writer, OrderDirection value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override OrderDirection ReadJson(JsonReader reader, Type objectType, OrderDirection existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return OrderDirection.FromName(name);
        }
    }


    [JsonConverter(typeof(OrderDirectionConverter))]
    public class OrderDirection : Enumeration
    {
        public static OrderDirection Buy = new OrderDirection(1, "Buy");
        public static OrderDirection Sell = new OrderDirection(2, "Sell");

        protected OrderDirection()
        {
        }

        public OrderDirection(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderDirection> List()
        {
            return new[] { Buy, Sell };
        }

        public static OrderDirection FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(OrderDirection).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderDirection From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(OrderDirection).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
