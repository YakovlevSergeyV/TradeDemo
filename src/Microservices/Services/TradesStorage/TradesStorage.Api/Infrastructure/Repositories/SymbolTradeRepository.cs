namespace TradesStorage.Api.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Infrastructure.Common.Utils;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using GlobalSpace.Common.Guardly;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Options;
    using TradesStorage.Infrastructure;
    using TradesStorage.Infrastructure.Dto;
    using TradesStorage.Infrastructure.EntityContext;
    using TradesStorage.Infrastructure.UpgradeDb;

    public class SymbolTradeRepository : ISymbolTradeRepository
    {
        private readonly IOptions<SettingsApplication> settings;
        private readonly IDesignTimeDbContextFactory<TradeContext> tradeContextFactory;
        private readonly Func<IOptionsSqLite, IExecutionContextFactory> executionContextFactory;
        private readonly Func<IExecutionContextFactory, IOptionsSqLite, IDatabaseCreator<ICommandsDbTrade>>
            databaseCreatorTradeFactory;

        private readonly IFileSystemService fileSystemService;

        public SymbolTradeRepository(
            IOptions<SettingsApplication> settings
            , IDesignTimeDbContextFactory<TradeContext> tradeContextFactory
            , Func<IOptionsSqLite, IExecutionContextFactory> executionContextFactory
            , Func<IExecutionContextFactory, IOptionsSqLite, IDatabaseCreator<ICommandsDbTrade>>
                databaseCreatorTradeFactory
            , IFileSystemService fileSystemService)
        {
            Guard.Argument(() => settings, Is.NotNull);
            Guard.Argument(() => tradeContextFactory, Is.NotNull);
            Guard.Argument(() => executionContextFactory, Is.NotNull);
            Guard.Argument(() => databaseCreatorTradeFactory, Is.NotNull);
            Guard.Argument(() => fileSystemService, Is.NotNull);

            this.settings = settings;
            this.tradeContextFactory = tradeContextFactory;
            this.executionContextFactory = executionContextFactory;
            this.databaseCreatorTradeFactory = databaseCreatorTradeFactory;
            this.fileSystemService = fileSystemService;
        }

        public async Task<IEnumerable<Symbol>> GetAllCurrencyPairTradeAsync()
        {
            return await Task.Run(() => GetAllCurrencyPair(NamesDatabase.SearchTradePattern()));
        }

        public async Task CreateCurrencyPairTradeAsync(Symbol symbol)
        {
            Guard.Argument(() => symbol, Is.NotNull);

            await Task.Run(() => CreateCurrencyPairTrade(symbol));
        }

        public async Task DeleteCurrencyPairTradeAsync(Symbol symbol)
        {
            Guard.Argument(() => symbol, Is.NotNull);

            await DeleteCurrencyPairAsync(symbol, NamesDatabase.GetNameTrade);
        }

        public async Task<bool> ExistsCurrencyPairTradeAsync(Symbol symbol)
        {
            Guard.Argument(() => symbol, Is.NotNull);

            return await ExistsCurrencyPairAsync(symbol, NamesDatabase.GetNameTrade);
        }

        public async Task<long> GetDatabaseLength(Symbol symbol)
        {
            var name = NamesDatabase.GetNameTrade(symbol.ExchangeName, symbol.CurrencyPairName);
            var fullFileName = fileSystemService.PathCombine(settings.Value.DatabaseDir, name);

            return await Task.Run(() => fileSystemService.GetFileLength(fullFileName));
        }

        private IEnumerable<Symbol> GetAllCurrencyPair(string searchPattern)
        {
            return (from fullFileName in fileSystemService.GetFiles(settings.Value.DatabaseDir, searchPattern)
                select fileSystemService.GetFileNameWithoutExtension(fullFileName)
                into fileName
                select fileName.Split("_")
                into values
                where values.Length == 3
                select new Symbol {ExchangeName = values[1], CurrencyPairName = values[2]}).ToList();
        }

        private void CreateCurrencyPairTrade(Symbol symbol)
        {
            var fileName = NamesDatabase.GetNameTrade(symbol.ExchangeName, symbol.CurrencyPairName);
            var options = new OptionsSqLite(settings.Value.DatabaseDir, fileName);

            var databaseCreator = databaseCreatorTradeFactory(executionContextFactory(options), options);
            databaseCreator.UpgradeDb();

            var context = tradeContextFactory.CreateDbContext(new[] {options.GetConnectionString()});

            context.TradeInfos.Add(new TradeInfo
            {
                Guid = Guid.NewGuid().ToString("N"),
                Exchange = symbol.ExchangeName.ToUpper(),
                Symbol = symbol.CurrencyPairName.ToUpper()
            });
            context.SaveChanges();
        }

        private async Task DeleteCurrencyPairAsync(Symbol symbol, Func<string, string, string> name)
        {
            var fileName = name.Invoke(symbol.ExchangeName, symbol.CurrencyPairName);
            var fullFileName = fileSystemService.PathCombine(settings.Value.DatabaseDir, fileName);

            await Task.Run(() =>
            {
                if (fileSystemService.FileExists(fullFileName)) fileSystemService.FileDelete(fullFileName);
            });
        }

        private async Task<bool> ExistsCurrencyPairAsync(Symbol symbol, Func<string, string, string> name)
        {
            var fileName = name.Invoke(symbol.ExchangeName, symbol.CurrencyPairName);
            var fullFileName = fileSystemService.PathCombine(settings.Value.DatabaseDir, fileName);

            return await Task.Run(() => fileSystemService.FileExists(fullFileName));
        }
    }
}
