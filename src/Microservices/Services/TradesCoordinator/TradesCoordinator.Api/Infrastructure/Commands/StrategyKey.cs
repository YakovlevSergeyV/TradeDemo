namespace TradesCoordinator.Api.Infrastructure.Commands
{
    using EventBus.Events;
    using Microservices.Common.Infrastructure.StorageCommandEvent;
    using TradesCoordinator.Api.IntegrationEvents.Events;

    public class StrategyKey : IStrategyKey
    {
        public string Key(IntegrationEvent @event)
        {
            if (@event is TradesInsertedIntegrationEvent et)
            {
                return GetKey(et.ExchangeName, et.CurrencyPairName);
            }

            throw new System.NotImplementedException($"Нет реализации для события {@event}");
        }

        private static string GetKey(string exchange, string pair)
        {
            return $"{exchange.ToUpper()}-{pair.ToUpper()}";
        }
    }
}
