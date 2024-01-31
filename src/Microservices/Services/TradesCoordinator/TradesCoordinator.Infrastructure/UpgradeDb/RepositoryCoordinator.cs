namespace TradesCoordinator.Infrastructure.UpgradeDb
{
    using System;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using Microsoft.Data.Sqlite;
    using TradesCoordinator.Infrastructure.UpgradeDb.Queries;

    public class RepositoryCoordinator : RepositoryBase
    {
        public RepositoryCoordinator(ICommandsDbCoordinator commandsDb) : base(commandsDb)
        {
        }

        protected override void UpgradeInner(IExecutionContext executionContext)
        {
            var currentVersion = GetSchemaVersion(executionContext);
            if (currentVersion < new Version(1, 0, 1, 0)) UpgradeTo_1_0_1_0(executionContext);
        }

        private void UpgradeTo_1_0_1_0(IExecutionContext executionContext)
        {
            var sql = QueryManager.GetQueryUpgradeTo_1_0_1_0();
            using (var dbCommand = new SqliteCommand(sql))
            {
                dbCommand.CommandTimeout = 1200;
                executionContext.ExecuteScalar(dbCommand);
            }
        }
    }
}
