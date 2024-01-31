namespace TradesStorage.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TradesStorage.Infrastructure.Dto;

    public interface ITradeRepository
    {
        Task<TradeInfo> GetInfoAsync();
        Task<IEnumerable<Trade>> GetAsync(long timestampMin, long timestampMax);

        Task<bool> InsertAsync(IEnumerable<Trade> trades, long timestamp);
        Task UpdateTimestampAsync(long timestamp);
    }
}
