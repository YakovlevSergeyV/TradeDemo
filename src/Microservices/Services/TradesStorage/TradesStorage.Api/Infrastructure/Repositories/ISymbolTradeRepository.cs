namespace TradesStorage.Api.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TradesStorage.Infrastructure.Dto;

    public interface ISymbolTradeRepository
    {
        Task<IEnumerable<Symbol>> GetAllCurrencyPairTradeAsync();
        Task CreateCurrencyPairTradeAsync(Symbol symbol);
        Task DeleteCurrencyPairTradeAsync(Symbol symbol);
        Task<bool> ExistsCurrencyPairTradeAsync(Symbol symbol);
        Task<long> GetDatabaseLength(Symbol symbol);
    }
}
