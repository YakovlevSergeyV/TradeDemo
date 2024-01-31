namespace TradesStorage.Infrastructure.UpgradeDb
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Guardly;
    using TradesStorage.Infrastructure.Dto;

    public class TradeInfoMapper
        : MapperBase<TradeInfo>
    {

        private readonly ICommandsDbTrade _commandsDb;

        public TradeInfoMapper(ICommandsDbTrade commandsDb)
        {
            Guard.Argument(() => commandsDb, Is.NotNull);

            _commandsDb = commandsDb;
        }

        public void Update(IExecutionContext executionContext, TradeInfo candleInfo)
        {
            ExecuteNonQueryInner(
                () => _commandsDb.UpdateTradeInfo(candleInfo),
                executionContext);
        }

        protected ILoader<TradeInfo> GetLoader(Func<IDbCommand> commandFunc)
        {
            return new Loader<TradeInfo>(commandFunc, Load);
        }

        private TradeInfo Load(IReaderWrapper wrapper)
        {
            return new TradeInfo
            {
                Guid = wrapper.Read<string>("Guid"),
                Exchange = wrapper.Read<string>("Exchange"),
                Symbol = wrapper.Read<string>("CurrencyPair"),
                TimestampMin = wrapper.Read<long>("TimestampMin"),
                TimestampMax = wrapper.Read<long>("TimestampMax")
            };
        }
    }
}
