namespace TradesStorage.Api.Infrastructure.Services
{
    using System.Threading.Tasks;
    using EventBus.Abstractions;
    using GlobalSpace.Common.Guardly;
    using TradesStorage.Api.Infrastructure.Repositories;
    using TradesStorage.Api.IntegrationEvents.Events;
    using TradesStorage.Infrastructure.Dto;

    public class SymbolTradeService : ISymbolTradeService
    {
        private readonly IEventBus eventBus;
        private readonly ISymbolTradeRepository symbolTradeRepository;

        public SymbolTradeService(
            ISymbolTradeRepository symbolTradeRepository
            , IEventBus eventBus)
        {
            Guard.Argument(() => symbolTradeRepository, Is.NotNull);
            Guard.Argument(() => eventBus, Is.NotNull);

            this.symbolTradeRepository = symbolTradeRepository;
            this.eventBus = eventBus;
        }

        public async Task CreateCurrencyPairTradeAsync(Symbol symbol)
        {
            Guard.Argument(() => symbol, Is.NotNull);

            await symbolTradeRepository.CreateCurrencyPairTradeAsync(symbol);

            PublishNewCurrencyPairTradeIntegrationEvent(symbol);
        }

        public async Task DeleteCurrencyPairTradeAsync(Symbol symbol)
        {
            Guard.Argument(() => symbol, Is.NotNull);

            var exists = await symbolTradeRepository.ExistsCurrencyPairTradeAsync(symbol);
            if (!exists) return;

            await symbolTradeRepository.DeleteCurrencyPairTradeAsync(symbol);

            PublishDeleteCurrencyPairTradeIntegrationEvent(symbol);
        }

        private void PublishNewCurrencyPairTradeIntegrationEvent(Symbol symbol)
        {
            var @event = new CurrencyPairTradeCreatedIntegrationEvent(symbol.ExchangeName, symbol.CurrencyPairName);
            eventBus.Publish(@event);
        }

        private void PublishDeleteCurrencyPairTradeIntegrationEvent(Symbol symbol)
        {
            var @event = new CurrencyPairTradeDeletedIntegrationEvent(symbol.ExchangeName, symbol.CurrencyPairName);
            eventBus.Publish(@event);
        }
    }
}
