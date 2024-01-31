namespace TradesStorage.Api.Infrastructure.UpgradeDb
{
    using System;
    using System.Linq;
    using global::Infrastructure.Common.Utils;
    using GlobalSpace.Common.Guardly;
    using TradesStorage.Infrastructure;
    using TradesStorage.Infrastructure.Dto;
    using TradesStorage.Infrastructure.UpgradeDb;

    public class DatabaseUpgrade : IDatabaseUpgrade
    {
        private readonly string path;
        private readonly IFileSystemService fileSystemService;
        private readonly Func<OptionsDatabase, Symbol, IDatabaseCreatorFactory> factory;

        public DatabaseUpgrade(
            string path
            , Func<OptionsDatabase, Symbol,  IDatabaseCreatorFactory> factory
            , IFileSystemService fileSystemService)
        {
            Guard.Argument(() => path, Is.NotNullOrEmpty);
            Guard.Argument(() => fileSystemService, Is.NotNull);
            Guard.Argument(() => factory, Is.NotNull);

            this.path = path;
            this.fileSystemService = fileSystemService;
            this.factory = factory;
        }

        public void Run()
        {
            var files = fileSystemService.GetFiles(path, NamesDatabase.SearchTradePattern()).ToList();

            if (!files.Any()) return;

            foreach (var file in files)
            {
                UpgradeDb(file);
            }
          //  Parallel.ForEach(files, UpgradeDb);
        }

        private void UpgradeDb(string fullFileName)
        {
            var filename = fileSystemService.GetFileName(fullFileName);
            var type = filename.Split("_").FirstOrDefault();
            
            switch (type)
            {
                case NamesDatabase.TableTrade:
                    try
                    {
                        var options = new OptionsDatabase {FullFileName = fullFileName};
                        var databaseCreatorTrade = factory.Invoke(options, null).CreateTrade();
                        databaseCreatorTrade.UpgradeDb();
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                default:
                    throw new NotImplementedException($"Нет реализации для типа: {type}");
            }
        }
    }
}
