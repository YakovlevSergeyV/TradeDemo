namespace TradesStorage.Api.Infrastructure.EntityContext
{
    using GlobalSpace.Common.Dal.SQLite.Model;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Options;
    using TradesStorage.Infrastructure;
    using TradesStorage.Infrastructure.Dto;
    using TradesStorage.Infrastructure.EntityContext;
    using TradesStorage.Infrastructure.UpgradeDb;

    public class ProviderTradeContext : IProviderTradeContext
    {
        public ProviderTradeContext(
            Symbol symbol
            , IOptions<SettingsApplication> settings
            , IDesignTimeDbContextFactory<TradeContext> contextFactory)
        {
            var fileName = NamesDatabase.GetNameTrade(symbol.ExchangeName, symbol.CurrencyPairName);
            var options = new OptionsSqLite(settings.Value.DatabaseDir, fileName);

            Context = contextFactory.CreateDbContext(new[] { options.GetConnectionString() });
        }

       public TradeContext Context { get; }
    }
}
