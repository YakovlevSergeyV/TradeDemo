namespace TradesCoordinator.Api.Services
{
    using System.Threading.Tasks;
    using TradesCoordinator.Infrastructure.Dto;

    public interface IExchangeService
    {
        Task CreateExchangeAsync(ExchangeInfo exchange);
        Task UpdateExchangeAsync(ExchangeInfo exchange);
        Task DeleteExchangeAsync(ExchangeInfo exchange);
    }
}
