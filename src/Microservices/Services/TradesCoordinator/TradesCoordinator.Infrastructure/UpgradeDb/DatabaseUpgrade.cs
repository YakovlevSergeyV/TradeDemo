namespace TradesCoordinator.Infrastructure.UpgradeDb
{
    using System;
    using global::Infrastructure.Common.Utils;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using GlobalSpace.Common.Guardly;

    public class DatabaseUpgrade : IDatabaseUpgrade
    {
        private readonly string _path;
        private readonly IFileDb _fileDb;

        private readonly Func<IOptionsSqLite, IExecutionContextFactory> _executionContextFactory;

        private readonly Func<IExecutionContextFactory, IOptionsSqLite, IDatabaseCreator<ICommandsDbCoordinator>>
            _databaseCreatorFactory;

        public DatabaseUpgrade(
            string path
            , IFileDb fileDb
            , Func<IOptionsSqLite, IExecutionContextFactory> executionContextFactory
            , Func<IExecutionContextFactory, IOptionsSqLite, IDatabaseCreator<ICommandsDbCoordinator>>
                databaseCreatorFactory
            , IFileSystemService fileSystemService)
        {
            Guard.Argument(() => path, Is.NotNullOrEmpty);
            Guard.Argument(() => fileDb, Is.NotNull);
            Guard.Argument(() => fileSystemService, Is.NotNull);
            Guard.Argument(() => executionContextFactory, Is.NotNull);
            Guard.Argument(() => databaseCreatorFactory, Is.NotNull);

            _path = path;
            _fileDb = fileDb;
            _executionContextFactory = executionContextFactory;
            _databaseCreatorFactory = databaseCreatorFactory;
        }

        public void Run()
        {
            var options = new OptionsSqLite(_path, _fileDb.Name);

            var databaseCreator = _databaseCreatorFactory(_executionContextFactory(options), options);
            databaseCreator.UpgradeDb();
        }
    }
}
