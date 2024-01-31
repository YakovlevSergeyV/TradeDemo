namespace TradesCoordinator.Infrastructure.UpgradeDb
{
    using System;
    using global::Infrastructure.Common.Utils;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using GlobalSpace.Common.Guardly;

    public class DatabaseCreatorCoordinator : DatabaseCreator, IDatabaseCreator<ICommandsDbCoordinator>
    {
        private readonly ICommandsDbCoordinator _commandsDb;

        public DatabaseCreatorCoordinator(
            IExecutionContextFactory executionContextFactory,
            ICommandsDbCoordinator commandsDb,
            IFileSystemService fileSystemService,
            IRepository repository,
            IOptionsSqLite options) : base(() => { new DbCoordinator(executionContextFactory, commandsDb); })
        {
            Guard.Argument(() => commandsDb, Is.NotNull);
            Guard.Argument(() => executionContextFactory, Is.NotNull);
            Guard.Argument(() => fileSystemService, Is.NotNull);
            Guard.Argument(() => repository, Is.NotNull);
            Guard.Argument(() => options, Is.NotNull);

            _commandsDb = commandsDb;
            _executionContextFactory = executionContextFactory;
            _fileSystemService = fileSystemService;
            _repository = repository;
            _options = options;
        }

        public Version Version
        {
            get { return _commandsDb.Version; }
        }

        IDb IDatabaseCreator.CreateDb()
        {
            return CreateDb();
        }

        public IDb<ICommandsDbCoordinator> CreateDb()
        {
            return new DbCoordinator(_executionContextFactory, _commandsDb);
        }

    }
}
