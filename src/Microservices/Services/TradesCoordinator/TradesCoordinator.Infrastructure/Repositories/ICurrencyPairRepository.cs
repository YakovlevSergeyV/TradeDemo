namespace TradesCoordinator.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TradesCoordinator.Infrastructure.Dto;

    public interface ICurrencyPairRepository
    {
        Task<IEnumerable<SymbolInfo>> GetAllCurrencyPairAsync();
        Task<SymbolInfo> GetCurrencyPairAsync(string exchange, string symbol);
        Task<IEnumerable<SymbolInfo>> GetCurrencyPairsAsync(string exchange);
        Task<SymbolInfo> GetCurrencyPairAsync(string guid);

        Task CreateCurrencyPairAsync(SymbolInfo symbolInfo);
        Task UpdateCurrencyPairAsync(SymbolInfo symbolInfo);
        Task DeleteCurrencyPairAsync(SymbolInfo symbolInfo);

        Task<bool> ExistsCurrencyPairAsync(SymbolInfo symbolInfo);
        Task<bool> ExistsCurrencyPairAsync(string guid);
    }
}
