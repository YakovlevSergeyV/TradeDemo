namespace TradesStorage.Infrastructure.UpgradeDb
{
    using GlobalSpace.Common.Dal.SQLite.Abstract;

    public interface IDatabaseCreatorFactory
    {
        IDatabaseCreator<ICommandsDbTrade> CreateTrade();
    }
}
