namespace TradesStorage.Infrastructure.UpgradeDb
{
    using System.Data;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using TradesStorage.Infrastructure.Dto;

    public interface ICommandsDbTrade : ICommandsDb
    {
        IDbCommand CreateTradeTable();
        IDbCommand CreateTradeInfoTable();
        IDbCommand InsertTrade(Trade trade);
        IDbCommand UpdateTradeInfo(TradeInfo tradeInfo);
    }
}
