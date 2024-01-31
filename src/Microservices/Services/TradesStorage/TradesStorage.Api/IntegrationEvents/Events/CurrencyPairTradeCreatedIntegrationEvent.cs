namespace TradesStorage.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class CurrencyPairTradeCreatedIntegrationEvent : IntegrationEvent
    {
        public CurrencyPairTradeCreatedIntegrationEvent(string exchangeName, string currencyPairName)
        {
            ExchangeName = exchangeName;
            CurrencyPairName = currencyPairName;
        }

        public string ExchangeName { get; set; }
        public string CurrencyPairName { get; set; }
    }
}
