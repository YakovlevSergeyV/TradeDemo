namespace TradesStorage.Infrastructure.UpgradeDb
{
    using System;
    using global::Infrastructure.Common.Utils;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using GlobalSpace.Common.Guardly;
    using TradesStorage.Infrastructure.Dto;

    public class OptionsDatabase
    {
        public string DatabaseDir { get; set; }
        public string FullFileName { get; set; }
    }

    public class DatabaseCreatorFactory : IDatabaseCreatorFactory
    {
        private readonly OptionsDatabase _options;
        private readonly Symbol _symbol;
        private readonly IFileSystemService _fileSystemService;
        private readonly Func<IOptionsSqLite, IExecutionContextFactory> _executionContextFactory;

        private readonly Func<IExecutionContextFactory, IOptionsSqLite, IDatabaseCreator<ICommandsDbTrade>>
            _databaseCreatorTradeFactory;

        public DatabaseCreatorFactory(
            OptionsDatabase options
            , Symbol symbol
            , IFileSystemService fileSystemService
            , Func<IOptionsSqLite, IExecutionContextFactory> executionContextFactory
            , Func<IExecutionContextFactory, IOptionsSqLite, IDatabaseCreator<ICommandsDbTrade>>
                databaseCreatorTradeFactory)
        {
            Guard.Argument(() => options, Is.NotNull);
            Guard.Argument(() => fileSystemService, Is.NotNull);
            Guard.Argument(() => executionContextFactory, Is.NotNull);
            Guard.Argument(() => databaseCreatorTradeFactory, Is.NotNull);

            _options = options;
            _symbol = symbol;
            _fileSystemService = fileSystemService;
            _executionContextFactory = executionContextFactory;
            _databaseCreatorTradeFactory = databaseCreatorTradeFactory;
 }

        public IDatabaseCreator<ICommandsDbTrade> CreateTrade()
        {
            var optionsSqLite = GetOptionsSqLite(NamesDatabase.GetNameTrade);
            return _databaseCreatorTradeFactory(_executionContextFactory(optionsSqLite), optionsSqLite);
        }

        private OptionsSqLite GetOptionsSqLite(Func<string, string, string> name)
        {
            var fileName = string.Empty;

            if (_symbol != null)
            {
                fileName = name.Invoke(_symbol.ExchangeName, _symbol.CurrencyPairName);
            }
            else if (!string.IsNullOrEmpty(_options.FullFileName))
            {
                fileName = _fileSystemService.GetFileName(_options.FullFileName);
            }

            var path = string.Empty;
            if (!string.IsNullOrEmpty(_options.DatabaseDir))
            {
                path = _options.DatabaseDir;
            }
            else if (!string.IsNullOrEmpty(_options.FullFileName))
            {
                path = _fileSystemService.GetDirectoryPath(_options.FullFileName);
            }

            return new OptionsSqLite(path, fileName);
        }
    }
}
