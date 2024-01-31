namespace TradesStorage.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class TradesInsertedIntegrationEvent : IntegrationEvent
    {
        public TradesInsertedIntegrationEvent(string exchangeName, string currencyPairName)
        {
            ExchangeName = exchangeName;
            CurrencyPairName = currencyPairName;
        }

        public string ExchangeName { get; set; }
        public string CurrencyPairName { get; set; }
    }
}
