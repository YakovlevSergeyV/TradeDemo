namespace TradesCoordinator.Infrastructure.UpgradeDb
{
    using System.Data;
    using GlobalSpace.Common.Dal.SQLite.Abstract;

    public interface ICommandsDbCoordinator : ICommandsDb
    {
        IDbCommand CreateExchangesTable();
        IDbCommand CreateCurrencyPairsTable();
    }
}
