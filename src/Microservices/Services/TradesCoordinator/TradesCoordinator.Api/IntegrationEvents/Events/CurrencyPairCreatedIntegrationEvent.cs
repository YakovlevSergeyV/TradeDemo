namespace TradesCoordinator.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class CurrencyPairCreatedIntegrationEvent : IntegrationEvent
    {
        public CurrencyPairCreatedIntegrationEvent(string exchangeName, string currencyPairName)
        {
            ExchangeName = exchangeName;
            CurrencyPairName = currencyPairName;
        }

        public string ExchangeName { get; set; }
        public string CurrencyPairName { get; set; }
    }
}
