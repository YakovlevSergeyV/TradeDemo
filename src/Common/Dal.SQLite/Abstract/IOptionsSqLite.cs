namespace GlobalSpace.Common.Dal.SQLite.Abstract
{
    using System;

    public interface IOptionsSqLite
    {
        TimeSpan SqliteTimeout { get; }
        string DatabaseDir { get; }
        string FileNameDb { get; }
        string FullFileNameDb { get; }
        string GetConnectionString();
    }
}
