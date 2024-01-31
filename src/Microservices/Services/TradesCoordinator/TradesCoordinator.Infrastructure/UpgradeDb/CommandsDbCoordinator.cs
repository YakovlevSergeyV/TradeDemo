namespace TradesCoordinator.Infrastructure.UpgradeDb
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using Microsoft.Data.Sqlite;

    public class CommandsDbCoordinator : CommandsDb, ICommandsDbCoordinator
    {
        private readonly Version _version;

        public CommandsDbCoordinator()
        {
            // Первый октет - зарезервирован
            // Второй октет - произошло критичное изменение схемы БД
            // Третий октет - произошло не критичное изменение схемы БД
            // Четвёртый октет - зарезервирован
            _version = new Version(1, 0, 1, 0);
        }

        public override Version Version
        {
            get { return _version; }
        }

        public IDbCommand CreateExchangesTable()
        {
            return new SqliteCommand(
                "CREATE TABLE IF NOT EXISTS [Exchanges] ([GUID] TEXT PRIMARY KEY NOT NULL, [ExchangeName] TEXT NOT NULL, [HeartBeatCycleInMs] INTEGER NOT NULL, [TimestampInitial] INTEGER NOT NULL);");
        }

        public IDbCommand CreateCurrencyPairsTable()
        {
            return new SqliteCommand(
                "CREATE TABLE IF NOT EXISTS [CurrencyPairs] ([GUID] TEXT PRIMARY KEY NOT NULL, [CurrencyPairName] TEXT NOT NULL, [ExchangeName] TEXT NOT NULL, [TimestampInitial] INTEGER NOT NULL, [LastCount] INTEGER NOT NULL, [LastIntervalMin] INTEGER NOT NULL); ");
        }
    }
}
