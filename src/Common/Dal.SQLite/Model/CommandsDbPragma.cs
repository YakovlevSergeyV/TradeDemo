namespace GlobalSpace.Common.Dal.SQLite.Model
{
    using System.Data;
    using Microsoft.Data.Sqlite;

    public class CommandsDbPragma
    {
        public IDbCommand JournalModeMemory()
        {
            return new SqliteCommand("PRAGMA journal_mode = MEMORY;");
        }

        public IDbCommand SynchronousOff()
        {
            return new SqliteCommand("PRAGMA synchronous = OFF;");
        }

        public IDbCommand SynchronousOn()
        {
            return new SqliteCommand("PRAGMA synchronous = ON;");
        }
    }
}
