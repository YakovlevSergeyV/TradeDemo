namespace GlobalSpace.Common.Dal.SQLite.Abstract
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal.Abstract;

    public interface IDb
    {
        IExecutionContextFactory ExecutionContextFactory { get; }
        ICommandsDb CommandsDb { get; }

        void WriteAdditionalInfo(Func<IDataReader> dataReaderFunction);
    }

    public interface IDb<out TCommands> : IDb
        where TCommands : ICommandsDb
    {
        new TCommands CommandsDb { get; }
    }
}
