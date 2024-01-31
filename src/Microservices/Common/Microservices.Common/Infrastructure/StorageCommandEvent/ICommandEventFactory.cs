namespace Microservices.Common.Infrastructure.StorageCommandEvent
{
    using EventBus.Events;

    public interface ICommandEventFactory
    {
        ICommandEvent Create(IntegrationEvent @event);
    }
}
