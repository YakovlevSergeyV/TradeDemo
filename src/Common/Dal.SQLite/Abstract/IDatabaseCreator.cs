namespace GlobalSpace.Common.Dal.SQLite.Abstract
{
    using System;

    public interface IDatabaseCreator
    {
        Version Version { get; }
        IDb CreateDb();
        void UpgradeDb();
    }

    public interface IDatabaseCreator<out TCommands> : IDatabaseCreator
        where TCommands : ICommandsDb
    {
        new IDb<TCommands> CreateDb();
    }
}
