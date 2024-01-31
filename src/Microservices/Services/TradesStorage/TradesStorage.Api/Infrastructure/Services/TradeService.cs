namespace TradesStorage.Api.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EventBus.Abstractions;
    using global::Infrastructure.Common.Convert;
    using global::Infrastructure.Common.Extensions;
    using GlobalSpace.Common.Guardly;
    using Microsoft.Extensions.Logging;
    using TradesStorage.Api.IntegrationEvents.Events;
    using TradesStorage.Infrastructure.Dto;
    using TradesStorage.Infrastructure.EntityContext;
    using TradesStorage.Infrastructure.Repositories;
    using TradesStorage.Infrastructure.UpgradeDb;

    public class TradeService : ITradeService
    {
        private readonly Symbol symbol;
        private readonly ITradeRepository tradeRepository;
        private readonly IEventBus eventBus;
        private readonly ILogger<TradeService> logger;

        public TradeService(
            Symbol symbol
            , Func<Symbol, IProviderTradeContext> providerTradeContextFactory
            , Func<Symbol, TradeContext, ITradeRepository> tradeRepositoryFactory
            , IEventBus eventBus
            , ILogger<TradeService> logger)
        {
            Guard.Argument(() => symbol, Is.NotNull);
            Guard.Argument(() => tradeRepositoryFactory, Is.NotNull);
            Guard.Argument(() => eventBus, Is.NotNull);

            var providerContext = providerTradeContextFactory.Invoke(symbol);

            this.symbol = symbol;
            this.tradeRepository = tradeRepositoryFactory(symbol, providerContext.Context);
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public async Task InsertAsync(IEnumerable<Trade> trades, long timestamp)
        {
            Guard.Argument(() => trades, Is.NotNull);

            logger.LogTrace("Запуск импорта сделок в хранилище {0}-{1}", symbol.ExchangeName, symbol.CurrencyPairName);

            if (trades.Any())
            {
                var result = await tradeRepository.InsertAsync(trades, timestamp);
                if (result)
                {
                    PublishTradesActualInsertedIntegrationEvent(trades);

                    logger.LogInformation(
                        "Импорт сделок в хранилище {0}-{1}, дата нач:{2}, дата кон:{3}, count:{4}",
                        symbol.ExchangeName,
                        symbol.CurrencyPairName,
                        ConvertDateTime.UnixTimeStampMillisecondsToDateTime(trades.Min(x => x.Timestamp))
                            .UpToSecondsToString(),
                        ConvertDateTime.UnixTimeStampMillisecondsToDateTime(trades.Max(x => x.Timestamp))
                            .UpToSecondsToString(),
                        trades.Count());
                }
            }

            PublishInsertTradesIntegrationEvent();
        }

        private void PublishInsertTradesIntegrationEvent()
        {
            var @event =new TradesInsertedIntegrationEvent(symbol.ExchangeName, symbol.CurrencyPairName);
            eventBus.Publish(@event);
        }

        private void PublishTradesActualInsertedIntegrationEvent(IEnumerable<Trade> trades)
        {
            var @event =
                new TradesActualInsertedIntegrationEvent(symbol.ExchangeName, symbol.CurrencyPairName,
                    trades.Min(x => x.Timestamp), trades.Max(x => x.Timestamp));
            eventBus.Publish(@event);
        }
    }
}
