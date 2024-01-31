namespace TradesStorage.Api.Infrastructure.UpgradeDb
{
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using Microsoft.Extensions.Options;
    using TradesStorage.Api;
    using TradesStorage.Infrastructure;
    using TradesStorage.Infrastructure.Dto;

    public class OptionsSqLiteFactory : IOptionsSqLiteFactory
    {
        private readonly OptionsSqLite options;

        public OptionsSqLiteFactory(
            Symbol symbol,
            IOptions<SettingsApplication> settings)
        {
            var fileName = NamesDatabase.GetNameTrade(symbol.ExchangeName, symbol.CurrencyPairName);
            options = new OptionsSqLite(settings.Value.DatabaseDir, fileName);
        }

        public IOptionsSqLite Create()
        {
            return options;
        }
    }
}
