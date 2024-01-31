namespace Microservices.Common.Infrastructure.StorageCommandEvent
{
    using System.Collections.Concurrent;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using EventBus.Events;

    public class StorageCommand : IStorageCommand
    {
        private readonly ConcurrentDictionary<string, ICommandEvent> _commands;
        private readonly IStrategyKey _strategyKey;
        private readonly ICommandEventFactory _commandFactory;

        public StorageCommand(
            IStrategyKey strategyKey,
            ICommandEventFactory commandFactory)
        {
            _strategyKey = strategyKey;
            _commandFactory = commandFactory;

            _commands = new ConcurrentDictionary<string, ICommandEvent>();
        }

        public void Add(IntegrationEvent @event)
        {
            var key = _strategyKey.Key(@event);
            if (!_commands.ContainsKey(key))
            {
                _commands.GetOrAdd(key, _commandFactory.Create(@event));
            }

            DoWork(@event);
        }

        private void DoWork(IntegrationEvent @event)
        {
            Observable.Start(() => TryDoWork(@event), NewThreadScheduler.Default);
        }

        private void TryDoWork(IntegrationEvent @event)
        {
            var key = _strategyKey.Key(@event);
            _commands[key].Event = @event;
            _commands[key].Execute();
        }
    }
}
