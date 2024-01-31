namespace TradesCoordinator.Infrastructure.Synchronization
{
    using System;
    using System.Collections.Generic;
    using TradesCoordinator.Infrastructure.Dto;

    public interface ISynchronizationRepository
    {
        void Initialization();

        IEnumerable<ExchangeInfo> Exchanges { get; }

        ISynchronizationManagerExchange GetExchangeSynchronization(string exchange);

        void AddExchange(ExchangeInfo exchange);
        void UpdateExchange(ExchangeInfo exchange);
        void DeleteExchange(ExchangeInfo exchange);
        void AddSymbol(ExchangeInfo exchange, SymbolInfo symbolInfo, DateTime date);
        void DeleteSymbol(ExchangeInfo exchange, SymbolInfo symbolInfo);

        void AddService(Service service, DateTime date);
        bool ExistsService(Service serviceo);
        IEnumerable<Service> Services { get; }
    }
}
