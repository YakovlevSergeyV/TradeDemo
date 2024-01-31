namespace TradesStorage.Api.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TradesStorage.Infrastructure.Dto;

    public interface ITradeService
    {
        Task InsertAsync(IEnumerable<Trade> trades, long timestamp);
    }
}
