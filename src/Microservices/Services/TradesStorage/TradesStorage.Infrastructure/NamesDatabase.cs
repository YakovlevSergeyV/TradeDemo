
namespace TradesStorage.Infrastructure
{
    public static class NamesDatabase
    {
        public const string TableTrade = "TRADES";

        public static string GetNameTrade(string exchange, string currencyPair)
        {
            return $"{TableTrade}_{exchange.ToUpper()}_{currencyPair.ToUpper()}.db";
        }

        public static string SearchTradePattern()
        {
            return $"TRADES_*.db";
        }
    }
}
