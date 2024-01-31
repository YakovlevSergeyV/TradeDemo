namespace TradesReader.Api.Extensions
{
    using TradesReader.Api.Dto;

    public static class UrlsConfigMarketsOperationsExtensions
    {
        public static string GetMarketTrades(this UrlsConfig urls, string exchange, string symbol, long timestampMin, long timestampMax) =>
            $"{urls.GetVersion1()}/Markets/trades/{exchange}/{symbol}/{timestampMin}/{timestampMax}";
    }
}
