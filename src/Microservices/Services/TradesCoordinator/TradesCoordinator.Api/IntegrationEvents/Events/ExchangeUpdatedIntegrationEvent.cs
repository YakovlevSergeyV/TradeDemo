namespace TradesCoordinator.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class ExchangeUpdatedIntegrationEvent : IntegrationEvent
    {
        public ExchangeUpdatedIntegrationEvent(string guid, string exchangeName)
        {
            Guid = guid;
            ExchangeName = exchangeName;
        }

        public string Guid { get; set; }
        public string ExchangeName { get; set; }
    }
}
