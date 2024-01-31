namespace TradesReader.Api.Request
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TradesReader.Api.Dto;

    internal interface IRequest
    {

        #region Markets

        Task<IEnumerable<Trade>> GetMarketTradesAsync
        (
            string exchange,
            ParametersTrade parameters
        );

        #endregion

        #region TradesStorage

        Task<TradeInfo> GetTradeInfoAsync(string exchange, string symbol);

        Task TradeUpdateTimestampAsync(string exchange, string symbol, long timestamp);

        Task TradeInsertAsync(string exchange, string symbol, long timestamp, IEnumerable<Trade> trades);

        #endregion

        #region TradesCoordinator

        Task TradeRegisterServiceAsync(ServiceInfo serviceInfo);

        Task<SymbolNext> GetTradeNextSymbolsAsync(ServiceInfo serviceInfo);

        Task<bool> GetTradeServiceStartedAsync(ServiceInfo serviceInfo);

        Task TradePerformanceSuccessfulAsync(PerformanceSuccessful performance);

        Task TradePerformanceNotSuccessfulAsync(PerformanceNotSuccessful performance);

        Task<SymbolInfo> GetTradeCoordinatorSymbolAsync(string exchange, string symbol);

        Task TradeCoordinatorUpdateSymbolAsync(SymbolInfo symbolInfo);

        Task<ExchangeInfo> GetTradeExchangeInfoAsync(string exchange);

        #endregion
    }
}
