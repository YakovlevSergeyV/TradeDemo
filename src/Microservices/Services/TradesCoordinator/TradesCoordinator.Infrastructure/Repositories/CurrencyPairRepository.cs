
namespace TradesCoordinator.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Infrastructure.Common.Ambient;
    using GlobalSpace.Common.Guardly;
    using Microsoft.EntityFrameworkCore;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.EntityContext;
    using TradesCoordinator.Infrastructure.Synchronization;

    public class CurrencyPairRepository : ICurrencyPairRepository
    {
        private readonly ISynchronizationRepository _synchronizationRepository;
        private readonly CoordinatorContext _context;

        public CurrencyPairRepository(
            CoordinatorContext context
            , ISynchronizationRepository synchronizationRepository)
        {
            Guard.Argument(() => context, Is.NotNull);
            Guard.Argument(() => synchronizationRepository, Is.NotNull);

            _context = context;
            _synchronizationRepository = synchronizationRepository;
        }

        public async Task<IEnumerable<SymbolInfo>> GetAllCurrencyPairAsync()
        {
            return await _context.CurrencyPairs.ToListAsync();
        }

        public async Task<SymbolInfo> GetCurrencyPairAsync(string exchange, string symbol)
        {
            return await _context.CurrencyPairs.FirstOrDefaultAsync(x=>x.ExchangeName == exchange && x.CurrencyPairName == symbol);
        }

        public async Task<IEnumerable<SymbolInfo>> GetCurrencyPairsAsync(string exchange)
        {
            return await _context
                .CurrencyPairs
                .Where(x => x.ExchangeName == exchange)
                .ToListAsync();
        }

        public async Task<SymbolInfo> GetCurrencyPairAsync(string guid)
        {
            var items = await _context.CurrencyPairs.Where(x => x.Guid == guid).ToListAsync();
            return items.FirstOrDefault();
        }

        public async Task CreateCurrencyPairAsync(SymbolInfo symbolInfo)
        {
            Guard.Argument(() => symbolInfo, Is.NotNull);

            var exchanges = await _context.Exchanges.Where(x =>x.ExchangeName == symbolInfo.ExchangeName)
                .ToListAsync();
            var exchange = exchanges.FirstOrDefault();
            if (exchange == null) throw new ArgumentNullException("exchange == null");

            var currencyPairs = await _context.CurrencyPairs
                .Where(x => x.ExchangeName == symbolInfo.ExchangeName &&
                            x.CurrencyPairName == symbolInfo.CurrencyPairName)
                .ToListAsync();

            if (currencyPairs.Any()) return;

            await _context.CurrencyPairs.AddAsync(symbolInfo);

            await _context.SaveChangesAsync();

            _synchronizationRepository.AddSymbol(exchange, symbolInfo, TimeProvider.Current.UtcNow);
        }

        public async Task UpdateCurrencyPairAsync(SymbolInfo symbolInfo)
        {
            Guard.Argument(() => symbolInfo, Is.NotNull);

            var items = await _context.CurrencyPairs
                .Where(x => x.Guid == symbolInfo.Guid)
                .ToListAsync();

            var item = items.First();
            item.TimestampInitial = symbolInfo.TimestampInitial;
            item.LastCount = symbolInfo.LastCount;
            item.LastIntervalMin = symbolInfo.LastIntervalMin;

            _context.CurrencyPairs.Update(item);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCurrencyPairAsync(SymbolInfo symbolInfo)
        {
            var exchanges = await _context.Exchanges.Where(x =>x.ExchangeName == symbolInfo.ExchangeName )
                .ToListAsync();
            var exchange = exchanges.FirstOrDefault();

            var symbolInfoDb = await _context.CurrencyPairs.Where(x =>
                    x.ExchangeName == symbolInfo.ExchangeName && x.CurrencyPairName == symbolInfo.CurrencyPairName)
                .SingleOrDefaultAsync();

            _context.CurrencyPairs.Remove(symbolInfoDb);
            await _context.SaveChangesAsync();

            _synchronizationRepository.DeleteSymbol(exchange, symbolInfoDb);
        }

        public async Task<bool> ExistsCurrencyPairAsync(SymbolInfo symbolInfo)
        {
            Guard.Argument(() => symbolInfo, Is.NotNull);
            
            var items = await _context.CurrencyPairs
                .Where(x => x.ExchangeName == symbolInfo.ExchangeName &&
                            x.CurrencyPairName == symbolInfo.CurrencyPairName)
                .ToListAsync();

            return items.Any();
        }

        public async Task<bool> ExistsCurrencyPairAsync(string guid)
        {
            var items = await _context.CurrencyPairs
                .Where(x => x.Guid == guid)
                .ToListAsync();

            return items.Any();
        }
    }
}
