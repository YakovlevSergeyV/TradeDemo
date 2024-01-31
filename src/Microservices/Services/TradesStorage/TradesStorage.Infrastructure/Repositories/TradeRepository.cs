namespace TradesStorage.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Infrastructure.Common.Convert;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Guardly;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TradesStorage.Infrastructure.Dto;
    using TradesStorage.Infrastructure.EntityContext;
    using TradesStorage.Infrastructure.UpgradeDb;

    public class TradeRepository : ITradeRepository
    {
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IRepositoryTrade _repositoryDb;

        private readonly Symbol _symbol;
        private readonly TradeContext _context;
        private readonly ILogger<TradeRepository> _logger;

        public TradeRepository
        (
            Symbol symbol
            , TradeContext context
            , IRepositoryTrade repositoryDb
            , Func<Symbol, IOptionsSqLiteFactory> optionsSqLiteFactoryFactory
            , Func<IOptionsSqLite, IExecutionContextFactory> executionContextFactoryFactory
            , ILogger<TradeRepository> logger
        )
        {
            _symbol = symbol;
            _context = context;
            _repositoryDb = repositoryDb;
            _logger = logger;

            var optionsSqLite = optionsSqLiteFactoryFactory(symbol).Create();
            _executionContextFactory = executionContextFactoryFactory(optionsSqLite);
        }

        public async Task<TradeInfo> GetInfoAsync()
        {
            try
            {
                return await _context.TradeInfos.SingleOrDefaultAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    $"Ошибка чтения информации сделок по {_symbol.ExchangeName}-{_symbol.CurrencyPairName}.");
                throw;
            }
            finally
            {
                _context.CloseDb();
            }
        }

        public async Task<IEnumerable<Trade>> GetAsync(long timestampMin, long timestampMax)
        {
            try
            {
                return await _context.Trades
                .Where(x => x.Timestamp >= timestampMin && x.Timestamp <= timestampMax)
                .ToListAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    $"Ошибка чтения сделок по {_symbol.ExchangeName}-{_symbol.CurrencyPairName}.");
                throw;
            }
            finally
            {
                _context.CloseDb();
            }
        }

        public async Task<bool> InsertAsync(IEnumerable<Trade> trades, long timestamp)
        {
            _logger.LogInformation($"Старт записи сделок в количестве {trades.Count()} по {_symbol.ExchangeName}-{_symbol.CurrencyPairName}.");

            bool change;
            if (trades.Count() < 100)
            {
                change = await InsertInnerAsync(trades, timestamp);
            }
            else
            {
                change = await InsertBulkAsync(trades, timestamp);
            }

            _logger.LogInformation($"Финиш записи сделок в количестве {trades.Count()} по {_symbol.ExchangeName}-{_symbol.CurrencyPairName}.");

            return change;
        }

        public async Task UpdateTimestampAsync(long timestamp)
        {
            try
            {
                var tradeInfo = await _context.TradeInfos.SingleOrDefaultAsync();
                tradeInfo.TimestampMax = timestamp;

                _context.TradeInfos.Update(tradeInfo);

                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    $"Ошибка обновления максимальной даты сделок по {_symbol.ExchangeName}-{_symbol.CurrencyPairName}.");
                throw;
            }
            finally
            {
                _context.CloseDb();
            }
        }

        private async Task<bool> InsertInnerAsync(IEnumerable<Trade> trades, long timestamp)
        {
            Guard.Argument(() => trades, Is.NotNull);

            try
            {
                await _context.Database.BeginTransactionAsync();

                long idDb = 0;
                if (_context.Trades.Any())
                {
                    idDb = _context.Trades.Max(x => x.Id);
                }

                var tradeInfo = await _context.TradeInfos.SingleOrDefaultAsync();
                var tradesActual = trades.Where(x => x.Id > idDb).ToList();
                if (tradesActual.Any())
                {
                    await _context.Trades.AddRangeAsync(tradesActual);

                    if (tradeInfo.TimestampMin == 0)
                    {
                        tradeInfo.TimestampMin = tradesActual.Min(x => x.Timestamp);
                    }
                }

                var timestampMax = tradeInfo.TimestampMax;
                if (tradesActual.Any())
                {
                    timestampMax = tradesActual.Max(x => x.Timestamp);
                }
                else if (ConvertDateTime.CurrentDateToUnixTimeStampMilliseconds() - timestamp > 60000)
                {
                    var dt = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(timestamp);
                    timestampMax = ConvertDateTime.DateToUnixTimeStampMilliseconds(
                        new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, DateTimeKind.Utc));
                }

                tradeInfo.TimestampMax = timestampMax;

                _context.TradeInfos.Update(tradeInfo);

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();

                return tradesActual.Any();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    $"Ошибка записи сделок в количестве {trades.Count()} по {_symbol.ExchangeName}-{_symbol.CurrencyPairName}.");

                await _context.Database.RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _context.CloseDb();
            }
        }

        private async Task<bool> InsertBulkAsync(IEnumerable<Trade> trades, long timestamp)
        {
            var change = false;
            var tradeInfo = await _context.TradeInfos.SingleOrDefaultAsync();

            await Task.Run(() =>
            {
                using (var execution = _executionContextFactory.Create())
                {
                    execution.Open();
                    execution.BeginTransaction();
                    try
                    {

                        long idDb = 0;
                        if (_context.Trades.Any())
                        {
                            idDb = _context.Trades.Max(x => x.Id);
                        }

                        var tradesActual = trades.Where(x => x.Id > idDb).ToList();
                        if (tradesActual.Any())
                        {
                            _repositoryDb.InsertTrades(tradesActual, execution);

                            if (tradeInfo.TimestampMin == 0)
                            {
                                tradeInfo.TimestampMin = tradesActual.Min(x => x.Timestamp);
                            }
                        }

                        var timestampMax = tradeInfo.TimestampMax;
                        if (tradesActual.Any())
                        {
                            timestampMax = tradesActual.Max(x => x.Timestamp);
                        }
                        else if (ConvertDateTime.CurrentDateToUnixTimeStampMilliseconds() - timestamp > 60000)
                        {
                            var dt = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(timestamp);
                            timestampMax = ConvertDateTime.DateToUnixTimeStampMilliseconds(
                                new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, DateTimeKind.Utc));
                        }

                        tradeInfo.TimestampMax = timestampMax;
                        _repositoryDb.UpdateTradeInfo(tradeInfo, execution);

                        change = true;

                        execution.Commit();
                        execution.Close();
                    }
                    catch (Exception)
                    {
                        execution.Rollback();
                        execution.Close();

                        _logger.LogError(
                            $"Ошибка записи сделок в количестве {trades.Count()} по {_symbol.ExchangeName}-{_symbol.CurrencyPairName}.");

                        throw;
                    }
                }
            });

            return change;
        }
    }
}
