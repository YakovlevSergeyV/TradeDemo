namespace TradesStorage.Infrastructure.UpgradeDb
{
    using GlobalSpace.Common.Dal;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;

    public class DbTrade : DbBase, IDb<ICommandsDbTrade>
    {
        public DbTrade(
            IExecutionContextFactory executionContextFactory,
            ICommandsDbTrade commandsDb)
            : base(executionContextFactory, commandsDb)
        {
            CommandsDb = commandsDb;
            ExecutionContextHelper.Execute(
                ExecutionContextFactory,
                context =>
                {
                    CreateTable(context, commandsDb.CreateTradeInfoTable);
                    CreateTable(context, commandsDb.CreateTradeTable);
                });
        }

        ICommandsDb IDb.CommandsDb
        {
            get { return CommandsDb; }
        }

        public ICommandsDbTrade CommandsDb { get; private set; }
    }
}
