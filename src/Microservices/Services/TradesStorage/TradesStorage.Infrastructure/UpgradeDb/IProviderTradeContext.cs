namespace TradesStorage.Infrastructure.UpgradeDb
{
    using TradesStorage.Infrastructure.EntityContext;

    public interface IProviderTradeContext
    {
        TradeContext Context { get; }
    }
}
