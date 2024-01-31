namespace Microservices.Common.Infrastructure.StorageCommandEvent
{
    using EventBus.Events;

    public interface ICommandEvent
    {
        IntegrationEvent Event { get; set; }
        void Execute();
    }
}
