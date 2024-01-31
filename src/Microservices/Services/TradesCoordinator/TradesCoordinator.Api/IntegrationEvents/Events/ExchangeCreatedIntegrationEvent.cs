namespace TradesCoordinator.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class ExchangeCreatedIntegrationEvent : IntegrationEvent
    {
        public ExchangeCreatedIntegrationEvent(string exchangeName)
        {
            ExchangeName = exchangeName;
        }

        public string ExchangeName { get; set; }
    }
}
