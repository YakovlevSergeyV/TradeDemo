
namespace GlobalSpace.Common.Dal.SQLite.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Guardly;

    public class OptionsSqLite : IOptionsSqLite
    {
        private static readonly TimeSpan SqliteTimeoutDefault = TimeSpan.FromSeconds(300.0);

        public OptionsSqLite(string databaseDir, string fileNameDb) : this(databaseDir, fileNameDb,
            SqliteTimeoutDefault)
        {
        }

        public OptionsSqLite(string databaseDir, string fileNameDb, TimeSpan sqliteTimeout)
        {
            Guard.Argument(() => databaseDir, Is.NotNullOrEmpty);
            Guard.Argument(() => fileNameDb, Is.NotNullOrEmpty);

            DatabaseDir = databaseDir;
            SqliteTimeout = sqliteTimeout;
            FileNameDb = fileNameDb;
            FullFileNameDb = Path.Combine(DatabaseDir, FileNameDb);
        }

        public TimeSpan SqliteTimeout { get; private set; }
        public string DatabaseDir { get; private set; }
        public string FileNameDb { get; private set; }
        public string FullFileNameDb { get; private set; }

        public string GetConnectionString()
        {
            Guard.Argument(() => FullFileNameDb, Is.NotNullOrEmpty);

            return $"Data Source={FullFileNameDb}";
           // return $"Data Source={FullFileNameDb}; {GetSettings()}";
        }

        //private Dictionary<string, string> connectionSettings = new Dictionary<string, string>
        //{
        //    {"SchemaVersion", "3"},
        //    {"Journal Mode", "On"},
        //    {"Foreign Keys", "On"},
        //    {"Compress", "True"},
        //    {"PRAGMA page_size ", "4096"},
        //    {"PRAGMA encoding ", "UTF-8"},
        //    {"PRAGMA journal_mode", "TRUNCATE"},
        //    {"PRAGMA temp_store", "MEMORY"},
        //    {"PRAGMA synchronous", "NORMAL"},
        //};

        private Dictionary<string, string> connectionSettings = new Dictionary<string, string>
        {
            {"Version", "3"},
        };

        private string GetSettings()
        {
            var settings = connectionSettings.Aggregate(string.Empty,
                (current, row) =>
                    current + string.Format("{0}={1}; ", row.Key, row.Value));

            return settings.Trim().Substring(0, settings.Length - 2);
        }
    }
}
