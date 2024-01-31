namespace TradesCoordinator.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class CurrencyPairDeletedIntegrationEvent : IntegrationEvent
    {
        public CurrencyPairDeletedIntegrationEvent(string guid, string exchangeName, string currencyPairName)
        {
            Guid = guid;
            ExchangeName = exchangeName;
            CurrencyPairName = currencyPairName;
        }

        public string Guid { get; set; }
        public string ExchangeName { get; set; }
        public string CurrencyPairName { get; set; }
    }
}
