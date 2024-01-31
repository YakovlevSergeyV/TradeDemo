namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class OrderStatusConverter : JsonConverter<OrderStatus>
    {
        public override void WriteJson(JsonWriter writer, OrderStatus value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override OrderStatus ReadJson(JsonReader reader, Type objectType, OrderStatus existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return OrderStatus.FromName(name);
        }
    }

    [JsonConverter(typeof(OrderStatusConverter))]
    public class OrderStatus : Enumeration
    {
        /// <summary>
        /// Ордер создан. 
        /// </summary>
        public static OrderStatus Created = new OrderStatus(1, "Created");

        /// <summary>
        /// Ордер отправлен на биржу
        /// </summary>
        public static OrderStatus Submitted = new OrderStatus(2, "Submitted");

        /// <summary>
        /// Ордер принят биржей
        /// </summary>
        public static OrderStatus Accepted = new OrderStatus(3, "Accepted");

        /// <summary>
        /// Ордер выполенен частично
        /// </summary>
        public static OrderStatus Partial = new OrderStatus(4, "Partial");

        /// <summary>
        /// Ордер полностью заполнен
        /// </summary>
        public static OrderStatus Completed = new OrderStatus(5, "Completed");

        /// <summary>
        /// Ордер отменен
        /// </summary>
        public static OrderStatus Canceled = new OrderStatus(6, "Canceled");

        /// <summary>
        /// Ордер отклонен биржей
        /// </summary>
        public static OrderStatus Rejected = new OrderStatus(7, "Rejected");

        /// <summary>
        /// Ордер не удалось создать, например нет подключения к интернету или биржа не работает.
        /// </summary>
        public static OrderStatus Error = new OrderStatus(8, "Error");

        /// <summary>
        /// Истекший ордер.
        /// </summary>
        public static OrderStatus Expired = new OrderStatus(9, "Expired");

        private OrderStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderStatus> List()
        {
            return new[]
            {
                Created,
                Submitted,
                Accepted,
                Partial,
                Completed,
                Canceled,
                Rejected,
                Error,
                Expired
            };
        }

        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(OrderStatus).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(OrderStatus).Name}: {string.Join(",", List().Select(s => s.Id))}");
            }

            return state;
        }
    }
}
