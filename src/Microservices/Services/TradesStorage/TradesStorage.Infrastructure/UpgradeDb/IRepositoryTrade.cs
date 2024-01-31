namespace TradesStorage.Infrastructure.UpgradeDb
{
    using System.Collections.Generic;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using TradesStorage.Infrastructure.Dto;

    public interface IRepositoryTrade : IRepository
    {
        void InsertTrades(IEnumerable<Trade> trades, IExecutionContext executionContext);
        void UpdateTradeInfo(TradeInfo tradeInfo, IExecutionContext executionContext);
    }
}
