namespace TradesCoordinator.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class TradesInsertedIntegrationEvent : IntegrationEvent
    {
        public string ExchangeName { get; set; }
        public string CurrencyPairName { get; set; }
    }
}
