namespace TradesStorage.Infrastructure.UpgradeDb
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using GlobalSpace.Common.Dal;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Guardly;
    using TradesStorage.Infrastructure.Dto;

    public class TradeMapper
       : MapperBase<Trade>
    {

        private readonly ICommandsDbTrade _commandsDb;

        public TradeMapper(ICommandsDbTrade commandsDb)
        {
            Guard.Argument(() => commandsDb, Is.NotNull);

            _commandsDb = commandsDb;
        }

        public void Insert(IExecutionContext executionContext,
            IEnumerable<Trade> candles)
        {
            foreach (var candle in candles)
            {
                ExecuteNonQueryInner(
                    () => _commandsDb.InsertTrade(candle),
                    executionContext);
            }
        }

        protected ILoader<Trade> GetLoader(Func<IDbCommand> commandFunc)
        {
            return new Loader<Trade>(commandFunc, Load);
        }

        private Trade Load(IReaderWrapper wrapper)
        {
            return new Trade
            {
                Id = wrapper.Read<long>("Id"),
                Timestamp = wrapper.Read<long>("Timestamp"),
                Price = wrapper.Read<double>("Price"),
                Amount = wrapper.Read<double>("Amount")
            };
        }
    }
}
