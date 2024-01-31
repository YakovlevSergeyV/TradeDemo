namespace TradesStorage.Api.Infrastructure.AutofacModules
{
    using Autofac;
    using global::Infrastructure.Common.Providers;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using Microsoft.EntityFrameworkCore.Design;
    using TradesStorage.Api.Infrastructure.EntityContext;
    using TradesStorage.Api.Infrastructure.Repositories;
    using TradesStorage.Api.Infrastructure.Services;
    using TradesStorage.Api.Infrastructure.UpgradeDb;
    using TradesStorage.Infrastructure.EntityContext;
    using TradesStorage.Infrastructure.Repositories;
    using TradesStorage.Infrastructure.UpgradeDb;

    public class TradesStorageApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();
            builder.RegisterType<SymbolDictionary>().As<ISymbolDictionary>().SingleInstance();
            builder.RegisterType<DatabaseUpgrade>().As<IDatabaseUpgrade>();
            builder.RegisterType<SqLiteExecutionContextFactory>().As<IExecutionContextFactory>();
            builder.RegisterType<CommandsDbTrade>().As<ICommandsDbTrade>();
            builder.RegisterType<RepositoryTrade>().As<IRepositoryTrade>();
            builder.RegisterType<DbTrade>().As<IDb<ICommandsDbTrade>>();
            builder.RegisterType<DatabaseCreatorTrade>().As<IDatabaseCreator<ICommandsDbTrade>>();
            builder.RegisterType<DatabaseCreatorFactory>().As<IDatabaseCreatorFactory>();
            builder.RegisterType<OptionsSqLiteFactory>().As<IOptionsSqLiteFactory>();
            builder.RegisterType<SymbolTradeRepository>().As<ISymbolTradeRepository>();
            builder.RegisterType<SymbolTradeService>().As<ISymbolTradeService>();
            builder.RegisterType<TradeRepository>().As<ITradeRepository>();
            builder.RegisterType<TradeService>().As<ITradeService>();

            builder.RegisterType<TradeContextDesignFactory>().As<IDesignTimeDbContextFactory<TradeContext>>();
            builder.RegisterType<ProviderTradeContext>().As<IProviderTradeContext>();
        }
    }
}
