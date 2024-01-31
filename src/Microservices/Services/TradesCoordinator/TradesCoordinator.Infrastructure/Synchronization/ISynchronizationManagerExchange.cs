namespace TradesCoordinator.Infrastructure.Synchronization
{
    using System;
    using TradesCoordinator.Infrastructure.Dto;

    public interface ISynchronizationManagerExchange
    {
        void AddCurrencyPair(SymbolInfo symbolInfo, DateTime date);
        void DeleteCurrencyPair(SymbolInfo symbolInfo);
        void UpdateExchange(ExchangeInfo exchange);

        SymbolNext NextCurrencyPair(Service service);

        void SuccessfulPerformance(Service service, string currencyPair, long timeStamp);

        void NotSuccessfulPerformance(Service service, string currencyPair, string exception);

        void SavedStorage(string exchange, string currencyPair);

        DateTime LastDatePerformanceService(Service service);
    }
}
