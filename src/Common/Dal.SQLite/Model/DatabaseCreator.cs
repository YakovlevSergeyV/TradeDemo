namespace GlobalSpace.Common.Dal.SQLite.Model
{
    using System;
    using global::Infrastructure.Common.Utils;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;

    public abstract class DatabaseCreator
    {
        private readonly Action _createDb;
        protected IExecutionContextFactory _executionContextFactory;
        protected IOptionsSqLite _options;
        protected IRepository _repository;
        protected IFileSystemService _fileSystemService;

        protected DatabaseCreator(
            Action createDb)
        {
            _createDb = createDb;
        }

        public virtual void UpgradeDb()
        {
            if (_fileSystemService.FileExists(_options.FullFileNameDb))
            {
                if (_fileSystemService.GetFileLength(_options.FullFileNameDb) <= 0)
                {
                    _fileSystemService.FileDelete(_options.FullFileNameDb);
                }
            }
            if (_fileSystemService.FileExists(_options.FullFileNameDb))
            {
                try
                {
                    using (var context = _executionContextFactory.Create())
                    {
                        context.Open();
                        _repository.Upgrade(context);
                        context.Close();
                    }
                }
                catch (Exception exception)
                {
                   // logAdapter.Error(exception, string.Format("Ошибка проверки связи с БД {0}", options.FullFileNameDb));

                    var fileName = _options.FileNameDb;
                    var badFullFileName = _fileSystemService.PathCombine(_options.DatabaseDir,
                        string.Format("{0}.bad",
                            fileName.Replace(_fileSystemService.GetExtension(fileName), "")));
                    if (_fileSystemService.FileExists(badFullFileName)) _fileSystemService.FileDelete(badFullFileName);

                    _fileSystemService.MoveFile(_options.FullFileNameDb, badFullFileName);
                }
            }
            if (!_fileSystemService.FileExists(_options.FullFileNameDb))
            {
                _createDb();
            }
        }

    
    }
}
