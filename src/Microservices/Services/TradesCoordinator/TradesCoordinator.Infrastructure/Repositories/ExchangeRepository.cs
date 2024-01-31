namespace TradesCoordinator.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GlobalSpace.Common.Guardly;
    using Microsoft.EntityFrameworkCore;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.EntityContext;
    using TradesCoordinator.Infrastructure.Synchronization;

    public class ExchangeRepository : IExchangeRepository
    {
        private readonly ISynchronizationRepository _synchronizationRepository;
        private CoordinatorContext _context;

        public ExchangeRepository(CoordinatorContext context
            , ISynchronizationRepository synchronizationRepository)
        {
            Guard.Argument(() => context, Is.NotNull);
            Guard.Argument(() => synchronizationRepository, Is.NotNull);

            _context = context;
            _synchronizationRepository = synchronizationRepository;
        }

        public async Task<IEnumerable<ExchangeInfo>> GetAllExchangeAsync()
        {
            return await _context.Exchanges.ToListAsync();
        }

        public async Task<ExchangeInfo> GetExchangeAsync(string guid)
        {
            var items = await _context.Exchanges.Where(x => x.Guid == guid).ToListAsync();
            return items.FirstOrDefault();
        }

        public async Task<ExchangeInfo> GetExchangeByNameAsync(string exchange)
        {
            return await _context.Exchanges.FirstOrDefaultAsync(x => x.ExchangeName == exchange);
        }

        public async Task CreateExchangeAsync(ExchangeInfo exchange)
        {
            var exchanges = await _context.Exchanges.Where(x =>
                    x.ExchangeName == exchange.ExchangeName)
                .ToListAsync();
            if (exchanges.Any()) return;

            await _context.Exchanges.AddAsync(exchange);

            await _context.SaveChangesAsync();

            _synchronizationRepository.AddExchange(exchange);
        }

        public async Task UpdateExchangeAsync(ExchangeInfo exchange)
        {
            var items = await _context.Exchanges
                .Where(x => x.Guid == exchange.Guid)
                .ToListAsync();

            var item = items.First();

            item.HeartBeatCycleInMs = exchange.HeartBeatCycleInMs;

            item.TimestampInitial = exchange.TimestampInitial;

            _context.Exchanges.Update(item);

            await _context.SaveChangesAsync();

            _synchronizationRepository.UpdateExchange(exchange);
        }

        public async Task DeleteExchangeAsync(ExchangeInfo exchange)
        {
            var exchangeDb = await _context.Exchanges
                .Where(x => x.Guid == exchange.Guid)
                .FirstAsync();

            _context.Exchanges.Remove(exchangeDb);
            await _context.SaveChangesAsync();

            _synchronizationRepository.DeleteExchange(exchangeDb);
        }

        public async Task<bool> ExistsExchangeAsync(string guid)
        {
            var items = await _context.Exchanges
                .Where(x => x.Guid == guid)
                .ToListAsync();

            return items.Any();
        }

        public async Task<bool> ExistsExchangeAsync(ExchangeInfo exchange)
        {
            var items = await _context.Exchanges
                .Where(x => x.ExchangeName == exchange.ExchangeName)
                .ToListAsync();

            return items.Any();
        }
    }
}
