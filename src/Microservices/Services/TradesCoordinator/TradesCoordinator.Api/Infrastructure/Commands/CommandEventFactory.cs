namespace TradesCoordinator.Api.Infrastructure.Commands
{
    using EventBus.Events;
    using Microservices.Common.Infrastructure.StorageCommandEvent;
    using System;
    using TradesCoordinator.Api.IntegrationEvents.Events;

    public class CommandEventFactory : ICommandEventFactory
    {
        private readonly Func<TradesInsertedIntegrationEvent, ICommandEvent> _factory;

        public CommandEventFactory(Func<TradesInsertedIntegrationEvent, ICommandEvent> factory)
        {
            _factory = factory;
        }

        public ICommandEvent Create(IntegrationEvent @event)
        {
            return _factory.Invoke((TradesInsertedIntegrationEvent)@event);
        }
    }
}
