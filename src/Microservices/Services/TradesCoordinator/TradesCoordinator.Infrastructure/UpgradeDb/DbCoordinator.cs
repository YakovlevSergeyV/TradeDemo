namespace TradesCoordinator.Infrastructure.UpgradeDb
{
    using GlobalSpace.Common.Dal;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;

    public class DbCoordinator : DbBase, IDb<ICommandsDbCoordinator>
    {
        public DbCoordinator(
            IExecutionContextFactory executionContextFactory,
            ICommandsDbCoordinator commandsDb)
            : base(executionContextFactory, commandsDb)
        {
            CommandsDb = commandsDb;
            ExecutionContextHelper.Execute(
                ExecutionContextFactory,
                context =>
                {
                    CreateTable(context, commandsDb.CreateExchangesTable);
                    CreateTable(context, commandsDb.CreateCurrencyPairsTable);
                });
        }

        ICommandsDb IDb.CommandsDb
        {
            get { return CommandsDb; }
        }

        public ICommandsDbCoordinator CommandsDb { get; private set; }
    }
}
