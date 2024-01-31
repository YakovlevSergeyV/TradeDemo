namespace TradesStorage.Infrastructure.UpgradeDb
{
    using System;
    using System.Collections.Generic;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using TradesStorage.Infrastructure.Dto;

    public class RepositoryTrade : RepositoryBase, IRepositoryTrade
    {
        private readonly TradeMapper _tradeMapper;
        private readonly TradeInfoMapper _tradeInfoMapper;

        public RepositoryTrade(ICommandsDbTrade commandsDb) : base(commandsDb)
        {
            _tradeMapper = new TradeMapper(commandsDb);
            _tradeInfoMapper = new TradeInfoMapper(commandsDb);
        }
        protected override void UpgradeInner(IExecutionContext executionContext)
        {
            //var currentVersion = GetSchemaVersion(executionContext);
            //if (currentVersion < new Version(1, 0, 1, 0)) UpgradeTo_1_0_1_0(executionContext);
        }

        //private void UpgradeTo_1_0_1_0(IExecutionContext executionContext)
        //{
        //    var sql = QueryManager.GetQueryUpgradeTo_1_0_1_0();
        //    using (var dbCommand = new SqliteCommand(sql))
        //    {
        //        dbCommand.CommandTimeout = 1200;
        //        executionContext.ExecuteScalar(dbCommand);
        //    }
        //}

        public void InsertTrades(IEnumerable<Trade> trades, IExecutionContext executionContext)
        {
            _tradeMapper.Insert(executionContext, trades);
        }

        public void UpdateTradeInfo(TradeInfo tradeInfo, IExecutionContext executionContext)
        {
            _tradeInfoMapper.Update(executionContext, tradeInfo);
        }
    }
}
