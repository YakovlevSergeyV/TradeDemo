namespace TradesCoordinator.Api.IntegrationEvents.EventHandling
{
    using System.Threading.Tasks;
    using EventBus.Abstractions;
    using Microservices.Common.Infrastructure.StorageCommandEvent;
    using TradesCoordinator.Api.IntegrationEvents.Events;

    public class TradesInsertedIntegrationEventHandler : IIntegrationEventHandler<TradesInsertedIntegrationEvent>
    {
        private readonly IStorageCommand _storage;

        public TradesInsertedIntegrationEventHandler(IStorageCommand storage)
        {
            _storage = storage;
        }

        public async Task Handle(TradesInsertedIntegrationEvent @event)
        {
           _storage.Add(@event);
        }
    }
}
