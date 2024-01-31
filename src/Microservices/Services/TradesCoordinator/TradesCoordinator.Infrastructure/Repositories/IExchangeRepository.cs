namespace TradesCoordinator.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TradesCoordinator.Infrastructure.Dto;

    public interface IExchangeRepository
    {
        Task<IEnumerable<ExchangeInfo>> GetAllExchangeAsync();
        Task<ExchangeInfo> GetExchangeAsync(string guid);
        Task<ExchangeInfo> GetExchangeByNameAsync(string exchange);

        Task CreateExchangeAsync(ExchangeInfo exchange);
        Task UpdateExchangeAsync(ExchangeInfo exchange);
        Task DeleteExchangeAsync(ExchangeInfo exchange);

        Task<bool> ExistsExchangeAsync(string guid);
        Task<bool> ExistsExchangeAsync(ExchangeInfo exchange);
    }
}
