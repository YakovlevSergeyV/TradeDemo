namespace TradesReader.Api.Extensions
{
    using TradesReader.Api.Dto;

    public static class UrlsConfigTradeCoordinatorOperationsExtensions
    {
        public static string GetSymbol(this UrlsConfig urls, string exchange, string symbol)
            => $"{urls.GetVersion1()}/TradesCoordinatorSymbols/{exchange}/{symbol}";

        public static string GetNextsymbols(this UrlsConfig urls)
            => $"{urls.GetVersion1()}/TradesCoordinatorSymbols/nextSymbol";

        public static string GetUpdateSymbol(this UrlsConfig urls)
            => $"{urls.GetVersion1()}/TradesCoordinatorSymbols";

        public static string GetExchange(this UrlsConfig urls, string exchange)
            => $"{urls.GetVersion1()}/TradesCoordinatorExchanges/{exchange}";

        public static string GetServiceStarted(this UrlsConfig urls)
            => $"{urls.GetVersion1()}/TradesCoordinatorService/started";

        public static string GetPerformanceNotSuccessful(this UrlsConfig urls)
            => $"{urls.GetVersion1()}/TradesCoordinatorService/performance/notsuccessful";
      
        public static string GetPerformanceSuccessful(this UrlsConfig urls)
            => $"{urls.GetVersion1()}/TradesCoordinatorService/performance/successful";
        
        public static string GetRegisterService(this UrlsConfig urls)
            => $"{urls.GetVersion1()}/TradesCoordinatorService";
        
    }
}
