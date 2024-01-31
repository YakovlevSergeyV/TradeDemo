namespace TradesCoordinator.Api.Infrastructure.AutofacModules
{
    using Autofac;
    using global::Infrastructure.Common.Providers;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using Microservices.Common.Infrastructure.StorageCommandEvent;
    using TradesCoordinator.Api.Infrastructure.Commands;
    using TradesCoordinator.Api.IntegrationEvents.EventHandling;
    using TradesCoordinator.Api.Services;
    using TradesCoordinator.Infrastructure.Repositories;
    using TradesCoordinator.Infrastructure.Services;
    using TradesCoordinator.Infrastructure.Synchronization;
    using TradesCoordinator.Infrastructure.UpgradeDb;

    public class TradesCoordinatorApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();
            builder.RegisterType<DatabaseUpgrade>().As<IDatabaseUpgrade>();
            builder.RegisterType<CommandsDbCoordinator>().As<ICommandsDbCoordinator>();
            builder.RegisterType<SqLiteExecutionContextFactory>().As<IExecutionContextFactory>();
            builder.RegisterType<RepositoryCoordinator>().As<IRepository>();
            builder.RegisterType<DbCoordinator>().As<IDb<ICommandsDbCoordinator>>();
            builder.RegisterType<DatabaseCreatorCoordinator>().As<IDatabaseCreator<ICommandsDbCoordinator>>();
            builder.RegisterType<FileDb>().As<IFileDb>().SingleInstance();

            builder.RegisterType<SynchronizationManager>().As<ISynchronizationManager>().SingleInstance();
            builder.RegisterType<SynchronizationRepository>().As<ISynchronizationRepository>().SingleInstance();
            builder.RegisterType<SymbolDictionary>().As<ISymbolDictionary>().SingleInstance();

            builder.RegisterType<CurrencyPairService>().As<ICurrencyPairService>();
            builder.RegisterType<ExchangeService>().As<IExchangeService>();
            builder.RegisterType<CurrencyPairRepository>().As<ICurrencyPairRepository>();
            builder.RegisterType<ExchangeRepository>().As<IExchangeRepository>();

            builder.RegisterType<TradesInsertedIntegrationEventHandler>();
            builder.RegisterType<CommandEventFactory>().As<ICommandEventFactory>();
            builder.RegisterType<StrategyKey>().As<IStrategyKey>();
            builder.RegisterType<CommandEvent>().As<ICommandEvent>();
            builder.RegisterType<StorageCommand>().As<IStorageCommand>().SingleInstance();
        }
    }
}
