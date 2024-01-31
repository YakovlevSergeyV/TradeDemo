namespace TradesCoordinator.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class ExchangeDeletedIntegrationEvent : IntegrationEvent
    {
        public ExchangeDeletedIntegrationEvent(string guid, string exchangeName)
        {
            Guid = guid;
            ExchangeName = exchangeName;
        }

        public string Guid { get; set; }
        public string ExchangeName { get; set; }
    }
}
