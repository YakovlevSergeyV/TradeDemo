namespace TradesStorage.Api.Application
{
    //public class TradeQueries : ITradeQueries
    //{
    //    private readonly string connectionString;

    //    public TradeQueries(
    //        TradeParameters parameters
    //        , IOptions<SettingsApplication> settings
    //    )
    //    {
    //        Guard.Argument(() => parameters, Is.NotNull);
    //        Guard.Argument(() => settings, Is.NotNull);

    //        connectionString = GetConnectionString(settings.Value.DatabaseDir, parameters.Exchange, parameters.CurrencyPair);
    //    }

    //    public async Task<TradeInfoViewModel> GetInfoAsync()
    //    {
    //        using (var connection = new SqliteConnection(connectionString))
    //        {
    //            connection.Open();

    //            var result = await connection.QueryAsync("SELECT * FROM [TradeInfo];");
    //            if (!result.Any())
    //            {
    //                return new TradeInfoViewModel();
    //            }
    //            return MapTradeInfo(result.First());
    //        }
    //    }

    //    public async Task<IEnumerable<TradeViewModel>> GetAsync(long timestampMin, long timestampMax)
    //    {
    //        using (var connection = new SqliteConnection(connectionString))
    //        {
    //            connection.Open();

    //            var result = await connection.QueryAsync(
    //                $"SELECT * FROM [Trade] WHERE Timestamp>={timestampMin} AND Timestamp<={timestampMax};");

    //            if (!result.Any())
    //            {
    //                return new List<TradeViewModel>();
    //            }
    //            return MapTrade(result);
    //        }
    //    }

    //    private static TradeInfoViewModel MapTradeInfo(IDictionary<string, object> value)
    //    {
    //        return new TradeInfoViewModel
    //        {
    //            Exchange = value["Exchange"].ToString(),
    //            CurrencyPair = value["CurrencyPair"].ToString(),
    //            TimestampMin = long.Parse(value["TimestampMin"].ToString()),
    //            TimestampMax = long.Parse(value["TimestampMax"].ToString())
    //        };
    //    }

    //    private static IEnumerable<TradeViewModel> MapTrade(IEnumerable<dynamic> values)
    //    {
    //        return values.Select(value => (IDictionary<string, object>) value)
    //            .Select(v => new TradeViewModel
    //            {
    //                Timestamp = long.Parse(v["Timestamp"].ToString()),
    //                Price = decimal.Parse(v["Price"].ToString()),
    //                Amount = decimal.Parse(v["Amount"].ToString())
    //            })
    //            .ToList();
    //    }

    //    private static string GetConnectionString(string path, string exchange, string currencyPair)
    //    {
    //        var fileName = NamesDatabase.GetNameTrade(exchange, currencyPair);
    //        var optionsSqLite = new OptionsSqLite(path, fileName);
    //        return optionsSqLite.GetConnectionString();
    //    }
    //}
}
