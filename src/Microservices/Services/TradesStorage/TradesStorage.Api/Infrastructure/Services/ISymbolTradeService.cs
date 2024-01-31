namespace TradesStorage.Api.Infrastructure.Services
{
    using System.Threading.Tasks;
    using TradesStorage.Infrastructure.Dto;

    public interface ISymbolTradeService
    {
        Task CreateCurrencyPairTradeAsync(Symbol symbol);
        Task DeleteCurrencyPairTradeAsync(Symbol symbol);
    }
}
