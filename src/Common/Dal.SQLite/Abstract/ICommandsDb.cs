namespace GlobalSpace.Common.Dal.SQLite.Abstract
{
    using System;
    using System.Data;

    public interface ICommandsDb
    {
        Version Version { get; }

        IDbCommand CreateAdditionalInfoTable();
        IDbCommand InsertAdditionalParam(string key, string value);
        IDbCommand UpdateAdditionalParam(string key, string value);
        IDbCommand DeleteAdditionalParam(string key);

        IDbCommand ExistsAdditionalParam(string key);
        IDbCommand GetAdditionalParam(string key);
    }
}
