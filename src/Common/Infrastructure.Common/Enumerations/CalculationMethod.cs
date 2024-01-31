namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class CalculationMethodConverter : JsonConverter<CalculationMethod>
    {
        public override void WriteJson(JsonWriter writer, CalculationMethod value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override CalculationMethod ReadJson(JsonReader reader, Type objectType, CalculationMethod existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return CalculationMethod.FromName(name);
        }
    }

    [JsonConverter(typeof(CalculationMethodConverter))]
    public class CalculationMethod : Enumeration
    {
        public static CalculationMethod ExhaustiveMethod = new CalculationMethod(1, "ExhaustiveMethod");
        public static CalculationMethod GeneticMethod = new CalculationMethod(2, "GeneticMethod");

        public CalculationMethod(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<CalculationMethod> List()
        {
            return new[]
            {
                ExhaustiveMethod,
                GeneticMethod
            };
        }

        public static CalculationMethod FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(CalculationMethod).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static CalculationMethod From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(CalculationMethod).Name}: {string.Join(",", List().Select(s => s.Id))}");
            }

            return state;
        }
    }
}
