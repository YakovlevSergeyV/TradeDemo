namespace Microservices.Common.Infrastructure.StorageCommandEvent
{
    using EventBus.Events;

    public interface IStorageCommand
    {
        void Add(IntegrationEvent @event);
    }
}
