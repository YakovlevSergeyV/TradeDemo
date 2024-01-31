namespace TradesStorage.Infrastructure.UpgradeDb
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using Microsoft.Data.Sqlite;
    using TradesStorage.Infrastructure.Dto;

    public class CommandsDbTrade : CommandsDb, ICommandsDbTrade
    {
        private readonly Version _version;

        public CommandsDbTrade()
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

        public IDbCommand CreateTradeInfoTable()
        {
            return new SqliteCommand(
                @"CREATE TABLE IF NOT EXISTS [TradeInfo] (
                        [Guid] TEXT PRIMARY KEY NOT NULL, 
                        [Exchange] TEXT NOT NULL, 
                        [Symbol] TEXT NOT NULL, 
                        [TimestampMin] INTEGER NOT NULL, 
                        [TimestampMax] INTEGER NOT NULL);
                        ");
        }

        public IDbCommand CreateTradeTable()
        {
            return new SqliteCommand(
                "CREATE TABLE IF NOT EXISTS [Trade] ([Id] INTEGER PRIMARY KEY NOT NULL, [Timestamp] INTEGER NOT NULL, [Price] NUMERIC NOT NULL, [Amount] NUMERIC NOT NULL); " +
                "CREATE INDEX IF NOT EXISTS [Trade_Timestamp] ON [Trade] (Timestamp);");
        }

        public IDbCommand InsertTrade(Trade trade)
        {
            var cmd =
                new SqliteCommand(
                    "INSERT INTO [Trade] ([Id], [Timestamp], [Price], [Amount]) " +
                    "VALUES (@id, @timestamp, @price, @amount);");
            cmd.Parameters.AddWithValue("@id", trade.Id);
            cmd.Parameters.AddWithValue("@timestamp", trade.Timestamp);
            cmd.Parameters.AddWithValue("@price", trade.Price);
            cmd.Parameters.AddWithValue("@amount", trade.Amount);
            return cmd;
        }

        public IDbCommand UpdateTradeInfo(TradeInfo tradeInfo)
        {
            var cmd = new SqliteCommand("UPDATE [TradeInfo] SET [TimestampMin]=@timestampMin, [TimestampMax]=@timestampMax");
            cmd.Parameters.AddWithValue("@timestampMin", tradeInfo.TimestampMin);
            cmd.Parameters.AddWithValue("@timestampMax", tradeInfo.TimestampMax);

            return cmd;
        }
    }
}
