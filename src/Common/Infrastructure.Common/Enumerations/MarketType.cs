namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MarketType : Enumeration
    {
        public static MarketType Spot = new MarketType(1, "spot");
        public static MarketType Future = new MarketType(2, "future");

        protected MarketType() { }

        public MarketType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<MarketType> List()
        {
            return new[] { Spot, Future };
        }

        public static MarketType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Возможные значения для {typeof(MarketType).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static MarketType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Возможные значения для {typeof(MarketType).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
