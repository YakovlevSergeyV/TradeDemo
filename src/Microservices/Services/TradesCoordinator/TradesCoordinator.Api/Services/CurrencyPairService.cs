namespace TradesCoordinator.Api.Services
{
    using System.Threading.Tasks;
    using EventBus.Abstractions;
    using GlobalSpace.Common.Guardly;
    using TradesCoordinator.Api.IntegrationEvents.Events;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.Repositories;

    public class CurrencyPairService : ICurrencyPairService
    {
        private readonly IEventBus _eventBus;
        private readonly ICurrencyPairRepository _currencyPairRepository;

        public CurrencyPairService(
            ICurrencyPairRepository currencyPairRepository
            , IEventBus eventBus)
        {
            Guard.Argument(() => currencyPairRepository, Is.NotNull);
            Guard.Argument(() => eventBus, Is.NotNull);

            _currencyPairRepository = currencyPairRepository;
            _eventBus = eventBus;
        }

        public async Task CreateCurrencyPairAsync(SymbolInfo symbolInfo)
        {
            Guard.Argument(() => symbolInfo, Is.NotNull);

            var exists = await _currencyPairRepository.ExistsCurrencyPairAsync(symbolInfo);
            if (exists) return;

            await _currencyPairRepository.CreateCurrencyPairAsync(symbolInfo);

            PublishNewCurrencyPairIntegrationEvent(symbolInfo);
        }

        public async Task UpdateCurrencyPairAsync(SymbolInfo symbolInfo)
        {
            Guard.Argument(() => symbolInfo, Is.NotNull);

            await _currencyPairRepository.UpdateCurrencyPairAsync(symbolInfo);

            PublishUpdateCurrencyPairIntegrationEvent(symbolInfo);
        }

        public async Task DeleteCurrencyPairAsync(SymbolInfo symbolInfo)
        {
            await _currencyPairRepository.DeleteCurrencyPairAsync(symbolInfo);
            PublishDeleteCurrencyPairIntegrationEvent(symbolInfo);
        }

        private void PublishNewCurrencyPairIntegrationEvent(SymbolInfo symbolInfo)
        {
            var @event = new CurrencyPairCreatedIntegrationEvent(
                symbolInfo.ExchangeName,
                symbolInfo.CurrencyPairName);
            _eventBus.Publish(@event);
        }

        private void PublishUpdateCurrencyPairIntegrationEvent(SymbolInfo symbolInfo)
        {
            var @event = new CurrencyPairUpdatedIntegrationEvent(
                symbolInfo.Guid,
                symbolInfo.ExchangeName,
                symbolInfo.CurrencyPairName);
            _eventBus.Publish(@event);
        }

        private void PublishDeleteCurrencyPairIntegrationEvent(SymbolInfo symbolInfo)
        {
            var @event = new CurrencyPairDeletedIntegrationEvent(
                symbolInfo.Guid,
                symbolInfo.ExchangeName,
                symbolInfo.CurrencyPairName);
            _eventBus.Publish(@event);
        }
    }
}
