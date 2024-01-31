namespace TradesCoordinator.Api.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using EventBus.Abstractions;
    using GlobalSpace.Common.Guardly;
    using TradesCoordinator.Api.IntegrationEvents.Events;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.Repositories;

    public class ExchangeService : IExchangeService
    {
        private readonly IEventBus _eventBus;
        private readonly IExchangeRepository _exchangeRepository;
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly ICurrencyPairService _currencyPairService;

        public ExchangeService(
            IEventBus eventBus
            , IExchangeRepository exchangeRepository
            , ICurrencyPairRepository currencyPairRepository
            , ICurrencyPairService currencyPairService)
        {
            Guard.Argument(() => eventBus, Is.NotNull);
            Guard.Argument(() => exchangeRepository, Is.NotNull);
            Guard.Argument(() => currencyPairRepository, Is.NotNull);
            Guard.Argument(() => currencyPairService, Is.NotNull);

            _eventBus = eventBus;
            _exchangeRepository = exchangeRepository;
            _currencyPairRepository = currencyPairRepository;
            _currencyPairService = currencyPairService;
        }

        public async Task CreateExchangeAsync(ExchangeInfo exchange)
        {
            Guard.Argument(() => exchange, Is.NotNull);

            var exists = await _exchangeRepository.ExistsExchangeAsync(exchange);
            if (exists) return;

            await _exchangeRepository.CreateExchangeAsync(exchange);

            PublishNewExchangeIntegrationEvent(exchange);
        }

        public async Task UpdateExchangeAsync(ExchangeInfo exchange)
        {
            Guard.Argument(() => exchange, Is.NotNull);

            await _exchangeRepository.UpdateExchangeAsync(exchange);

            PublishUpdateExchangeIntegrationEvent(exchange);
        }

        public async Task DeleteExchangeAsync(ExchangeInfo exchange)
        {
            Guard.Argument(() => exchange, Is.NotNull);

            var currencyPairs =
                await _currencyPairRepository.GetCurrencyPairsAsync(exchange.ExchangeName);

            if (currencyPairs.Any())
            {
                foreach (var currencyPair in currencyPairs)
                {
                    await _currencyPairService.DeleteCurrencyPairAsync(currencyPair);
                }
            }

            await _exchangeRepository.DeleteExchangeAsync(exchange);

            PublishDeleteExchangeIntegrationEvent(exchange);
        }
        
        private void PublishNewExchangeIntegrationEvent(ExchangeInfo exchange)
        {
            var @event = new ExchangeCreatedIntegrationEvent(
                exchange.ExchangeName);
            _eventBus.Publish(@event);
        }
   
        private void PublishUpdateExchangeIntegrationEvent(ExchangeInfo exchange)
        {
            var @event = new ExchangeUpdatedIntegrationEvent(
                exchange.Guid,
                exchange.ExchangeName);
            _eventBus.Publish(@event);
        }

        private void PublishDeleteExchangeIntegrationEvent(ExchangeInfo exchange)
        {
            var @event = new ExchangeDeletedIntegrationEvent(
                exchange.Guid,
                exchange.ExchangeName);
            _eventBus.Publish(@event);
        }
    }
}
