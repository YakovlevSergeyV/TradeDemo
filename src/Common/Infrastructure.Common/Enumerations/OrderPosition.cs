namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class OrderPositionListConverter : JsonConverter<IEnumerable<OrderPosition>>
    {
        public override void WriteJson(JsonWriter writer, IEnumerable<OrderPosition> value, JsonSerializer serializer)
        {
            writer.WriteValue(string.Join(",", value.Select(x => x.Name).ToList()));
        }

        public override IEnumerable<OrderPosition> ReadJson(JsonReader reader, Type objectType,
            IEnumerable<OrderPosition> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return string.IsNullOrEmpty(name)
                ? new List<OrderPosition>()
                : name.Split(',').Select(OrderPosition.FromName).ToList();
        }
    }

    public class OrderPositionConverter : JsonConverter<OrderPosition>
    {
        public override void WriteJson(JsonWriter writer, OrderPosition value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override OrderPosition ReadJson(JsonReader reader, Type objectType, OrderPosition existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return OrderPosition.FromName(name);
        }
    }


    [JsonConverter(typeof(OrderPositionConverter))]
    public class OrderPosition : Enumeration
    {
        /// <summary>
        /// Короткая позиция. 
        /// </summary>
        public static OrderPosition Short = new OrderPosition(1, "Short");
        /// <summary>
        /// Длинная позиция.
        /// </summary>
        public static OrderPosition Long = new OrderPosition(2, "Long");

        [JsonConstructor]
        private OrderPosition(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderPosition> List()
        {
            return new[]
            {
                Short,
                Long
            };
        }

        public static OrderPosition FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(OrderPosition).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderPosition From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(OrderPosition).Name}: {string.Join(",", List().Select(s => s.Id))}");
            }

            return state;
        }
    }
}
