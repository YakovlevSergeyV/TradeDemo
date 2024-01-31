namespace GlobalSpace.Common.Dal.SQLite.Model
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using Microsoft.Data.Sqlite;

    public abstract class CommandsDb : ICommandsDb
    {
        public abstract Version Version { get; }

        #region AdditionalInfo

        public IDbCommand CreateAdditionalInfoTable()
        {
            return
                new SqliteCommand(
                    "CREATE TABLE IF NOT EXISTS AdditionalInfo(InfoKey TEXT PRIMARY KEY NOT NULL, InfoValue TEXT NULL);");
        }

        public IDbCommand InsertAdditionalParam(string key, string value)
        {

            var cmd = new SqliteCommand("INSERT INTO AdditionalInfo ([InfoKey],[InfoValue]) " +
                                        "VALUES (@key,@value);");
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@value", value);

            return cmd;
        }

        public IDbCommand UpdateAdditionalParam(string key, string value)
        {
            var cmd = new SqliteCommand("UPDATE AdditionalInfo SET [InfoValue]=@value WHERE [InfoKey] = @key");
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@value", value);

            return cmd;
        }

        public IDbCommand DeleteAdditionalParam(string key)
        {
            var cmd = new SqliteCommand("DELETE FROM AdditionalInfo WHERE [InfoKey] = @key");
            cmd.Parameters.AddWithValue("@key", key);

            return cmd;
        }

        public IDbCommand ExistsAdditionalParam(string key)
        {
            var cmd = new SqliteCommand("SELECT COUNT(*) as cnt FROM AdditionalInfo WHERE [InfoKey] = @key");
            cmd.Parameters.AddWithValue("@key", key);

            return cmd;
        }

        public IDbCommand GetAdditionalParam(string key)
        {
            var cmd = new SqliteCommand("SELECT [InfoValue] FROM AdditionalInfo WHERE [InfoKey] = @key");
            cmd.Parameters.AddWithValue("@key", key);

            return cmd;
        }

        #endregion
    }
}
