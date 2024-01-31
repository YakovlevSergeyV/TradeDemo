namespace TradesCoordinator.Api.Services
{
    using System.Threading.Tasks;
    using TradesCoordinator.Infrastructure.Dto;

    public interface ICurrencyPairService
    {
        Task CreateCurrencyPairAsync(SymbolInfo symbolInfo);
        Task UpdateCurrencyPairAsync(SymbolInfo symbolInfo);
        Task DeleteCurrencyPairAsync(SymbolInfo symbolInfo);
    }
}
