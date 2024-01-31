namespace TradesReader.Api.Extensions
{
    using TradesReader.Api.Dto;

    public static class UrlsConfigTradesOperationsExtensions
    {
        public static string GetTradeInfo(this UrlsConfig urls, string exchange, string symbol)
            => $"{urls.GetVersion1()}/Trades/info/{exchange}/{symbol}";
        
        public static string GetUpdateTimestamp(this UrlsConfig urls, string exchange, string symbol, long timestamp)
            => $"{urls.GetVersion1()}/Trades/update/{exchange}/{symbol}/{timestamp}";

        public static string GetInsert(this UrlsConfig urls, string exchange, string symbol, long timestamp)
            => $"{urls.GetVersion1()}/Trades/insert/{exchange}/{symbol}/{timestamp}";
        
    }
}
