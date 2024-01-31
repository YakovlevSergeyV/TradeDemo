namespace TradesCoordinator.Infrastructure.Synchronization
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Infrastructure.Common.Ambient;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.Services;

    public class SynchronizationManager : ISynchronizationManager
    {
        private readonly ISynchronizationRepository _synchronizationRepository;
        private readonly ISymbolDictionary _symbolDictionary;

        public SynchronizationManager(
            ISynchronizationRepository synchronizationRepository,
            ISymbolDictionary symbolDictionary)
        {
            _synchronizationRepository = synchronizationRepository;
            _symbolDictionary = symbolDictionary;
        }

        public Task RegistrationService(Service service)
        {
            return Task.Run(() => { _synchronizationRepository.AddService(service, TimeProvider.Current.UtcNow); });
        }

        public Task<bool> StartedService(Service service)
        {
            return Task.Run(() =>
            {
                var result = _synchronizationRepository.ExistsService(service);
                if (!result) return false;

                var list = (from exchange in _synchronizationRepository.Exchanges.Where(x=>x.ExchangeName == service.ExchangeName)
                    select _synchronizationRepository.GetExchangeSynchronization(exchange.ExchangeName)
                    into exchangeSynchronization
                    where exchangeSynchronization != null
                    select exchangeSynchronization.LastDatePerformanceService(service)).ToList();

                if (!list.Any()) return false;

                var cycleInMs = _synchronizationRepository.Exchanges.First(x => x.ExchangeName == service.ExchangeName)
                    .HeartBeatCycleInMs;

                return list.Max(x => x).AddMilliseconds(cycleInMs * 2) > TimeProvider.Current.UtcNow;
            });
        }

        public IEnumerable<Service> Services => _synchronizationRepository.Services;

        public Task<SymbolNext> NextCurrencyPair(Service service)
        {
            return Task.Run(() =>
            {
                var exchangeSynchronization =
                    _synchronizationRepository.GetExchangeSynchronization(service.ExchangeName);
                return exchangeSynchronization?.NextCurrencyPair(service);
            });
        }

        public Task PerformanceSuccessful(PerformanceSuccessful request)
        {
            return Task.Run(() =>
            {
                var exchangeSynchronization =
                    _synchronizationRepository.GetExchangeSynchronization(request.ExchangeName);
                if (exchangeSynchronization == null) return;
                exchangeSynchronization.SuccessfulPerformance(request, request.CurrencyPairName, request.Timestamp);
            });
        }

        public Task PerformanceNotSuccessful(PerformanceNotSuccessful request)
        {
            return Task.Run(() =>
            {
                var exchangeSynchronization =
                    _synchronizationRepository.GetExchangeSynchronization(request.ExchangeName);
                if (exchangeSynchronization == null) return;
                exchangeSynchronization.NotSuccessfulPerformance(request, request.CurrencyPairName, request.Exception);
            });
        }

        public void SetDataStorage(string exchange, string currencyPair)
        {
            var exchangeSynchronization =
                _synchronizationRepository.GetExchangeSynchronization(exchange);
            exchangeSynchronization?.SavedStorage(exchange, currencyPair);
        }

        public void SwitchIn(Symbol symbol)
        {
            _symbolDictionary.DeleteSymbol(symbol);
        }

        public void SwitchOff(Symbol symbol)
        {
            _symbolDictionary.AddSymbol(symbol);
        }

        public bool GetRunning(Symbol symbol)
        {
            return !_symbolDictionary.Exists(symbol);
        }
    }
}
