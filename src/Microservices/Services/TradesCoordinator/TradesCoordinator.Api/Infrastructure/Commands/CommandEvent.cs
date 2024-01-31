namespace TradesCoordinator.Api.Infrastructure.Commands
{
    using System;
    using EventBus.Events;
    using Microservices.Common.Infrastructure.StorageCommandEvent;
    using Microsoft.Extensions.Logging;
    using TradesCoordinator.Api.IntegrationEvents.Events;
    using TradesCoordinator.Infrastructure.Synchronization;

    public class CommandEvent : ICommandEvent
    {
        private bool _execute;
        private readonly ISynchronizationManager _synchronizationManager;
        private readonly ILogger<CommandEvent> _logger;

        public CommandEvent(
            ISynchronizationManager synchronizationManager,
            ILogger<CommandEvent> logger)
        {
            _synchronizationManager = synchronizationManager;
            _logger = logger;

            _execute = false;
        }

        public IntegrationEvent Event { get; set; }

        public void Execute()
        {
            if (Event == null || _execute) return;
            _execute = true;
            var @event = (TradesInsertedIntegrationEvent)Event;

            try
            {
                _synchronizationManager.SetDataStorage(@event.ExchangeName, @event.CurrencyPairName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка изменения даты в координаторе.");
            }
            finally
            {
               _execute = false;
            }
        }
    }
}