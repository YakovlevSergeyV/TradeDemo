namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class WalletTypeConverter : JsonConverter<WalletType>
    {
        public override void WriteJson(JsonWriter writer, WalletType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override WalletType ReadJson(JsonReader reader, Type objectType, WalletType existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return WalletType.FromName(name);
        }
    }

    [JsonConverter(typeof(WalletTypeConverter))]
    public class WalletType
        : Enumeration
    {
        public static WalletType Exchange = new WalletType(1, "exchange");
        public static WalletType Deposit = new WalletType(2, "deposit");

        protected WalletType() { }

        public WalletType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<WalletType> List()
        {
            return new[] { Exchange, Deposit };
        }

        public static WalletType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Возможные значения для {typeof(WalletType).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static WalletType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Возможные значения для {typeof(WalletType).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
