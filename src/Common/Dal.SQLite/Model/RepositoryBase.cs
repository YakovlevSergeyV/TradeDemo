namespace GlobalSpace.Common.Dal.SQLite.Model
{
    using System;
    using global::Infrastructure.Common.Conversion;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Dtos;
    using GlobalSpace.Common.Dal.SQLite.Mappers;
    using GlobalSpace.Common.Guardly;

    public abstract class RepositoryBase : IRepository
    {
        private readonly AdditionalInfoMapper _additionalInfoMapper;
        private readonly ICommandsDb _commandsDb;
        private readonly CommandsDbPragma _commandsDbPragma;

        protected RepositoryBase(
            ICommandsDb commandsDb)
        {
            Guard.Argument(() => commandsDb, Is.NotNull);

            _commandsDb = commandsDb;

            _commandsDbPragma = new CommandsDbPragma();
            _additionalInfoMapper = new AdditionalInfoMapper(commandsDb);
        }

        public Version GetSchemaVersion(IExecutionContext executionContext)
        {
            Guard.Argument(() => executionContext, Is.NotNull);

            var version = GetAdditionalParam<Version>(AdditionalInfoKeys.SchemaVersion, null, executionContext);

            if (version == null)
                throw new InvalidOperationException("Невозможно определить версию схемы базы данных.");

            return version;
        }

       
        public void Upgrade(IExecutionContext executionContext)
        {
            Guard.Argument(() => executionContext, Is.NotNull);

            UpgradeInner(executionContext);
            SetAdditionalParam(AdditionalInfoKeys.SchemaVersion, _commandsDb.Version, executionContext);
        }

        protected virtual void UpgradeInner(IExecutionContext executionContext)
        {
        }

        public T GetAdditionalParam<T>(string key, T defaultValue, IExecutionContext executionContext)
        {
            Guard.Argument(() => key, Is.NotNullOrEmpty);
            Guard.Argument(() => executionContext, Is.NotNull);

            var dto = _additionalInfoMapper.Find(executionContext, key);
            if (dto == null)
                return defaultValue;

            T result;
            if (DataConversion.Convert(dto.InfoValue, out result))
                return result;

           // logAdapter.Warn("Не удалось преобразовать значение '{0}' к типу '{1}' для ключа '{2}'.", dto.InfoValue, typeof(T).FullName, key);
            return defaultValue;
        }

        public void SetAdditionalParam<T>(string key, T value, IExecutionContext executionContext)
        {
            Guard.Argument(() => key, Is.NotNullOrEmpty);
            Guard.Argument(() => executionContext, Is.NotNull);

            var additionalInfo = new AdditionalInfo
            {
                InfoKey = key,
                InfoValue = Convert.ToString(value)
            };

            var dto = _additionalInfoMapper.Find(executionContext, key);
            if (dto == null)
            {
                InsertAdditionalParam(additionalInfo, executionContext);
            }
            else
            {
                UpdateAdditionalParam(additionalInfo, executionContext);
            }
        }

        public void DeleteAdditionalParam(string key, IExecutionContext executionContext)
        {
            Guard.Argument(() => key, Is.NotNullOrEmpty);
            Guard.Argument(() => executionContext, Is.NotNull);

            _additionalInfoMapper.Delete(executionContext, key);
        }

        private void InsertAdditionalParam(AdditionalInfo additionalInfo, IExecutionContext executionContext)
        {
            _additionalInfoMapper.Insert(executionContext, additionalInfo);
        }

        private void UpdateAdditionalParam(AdditionalInfo additionalInfo, IExecutionContext executionContext)
        {
            _additionalInfoMapper.Update(executionContext, additionalInfo);
        }
        
        #region Pragma

        public void JournalModeMemory(IExecutionContext executionContext)
        {
            using (var command = _commandsDbPragma.JournalModeMemory())
                executionContext.ExecuteNonQuery(command);
        }

        public void SynchronousOff(IExecutionContext executionContext)
        {
            using (var command = _commandsDbPragma.SynchronousOff())
                executionContext.ExecuteNonQuery(command);
        }

        public void SynchronousOn(IExecutionContext executionContext)
        {
            using (var command = _commandsDbPragma.SynchronousOn())
                executionContext.ExecuteNonQuery(command);
        }

        #endregion
    }
}
