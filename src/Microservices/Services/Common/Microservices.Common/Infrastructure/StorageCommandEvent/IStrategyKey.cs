namespace Microservices.Common.Infrastructure.StorageCommandEvent
{
    using EventBus.Events;

    public interface IStrategyKey
    {
        string Key(IntegrationEvent @event);
    }
}
