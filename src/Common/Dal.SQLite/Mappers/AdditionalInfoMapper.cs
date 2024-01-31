namespace GlobalSpace.Common.Dal.SQLite.Mappers
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Dtos;
    using GlobalSpace.Common.Guardly;

    public class AdditionalInfoMapper
           : MapperBase<AdditionalInfo>
    {
        public AdditionalInfoMapper(ICommandsDb commandsDb)
        {
            Guard.Argument(() => commandsDb, Is.NotNull);

            _commandsDb = commandsDb;
        }

        public AdditionalInfo Find(IExecutionContext executionContext, string key)
        {
            Guard.Argument(() => executionContext, Is.NotNull);
            Guard.Argument(() => key, Is.NotNull);

            var loader = GetLoader(() => _commandsDb.GetAdditionalParam(key));

            return FindInner(loader, executionContext);
        }

        public void Insert(IExecutionContext executionContext, AdditionalInfo additionalInfoDto)
        {
            if (executionContext == null) throw new ArgumentNullException("executionContext");
            if (additionalInfoDto == null) throw new ArgumentNullException("additionalInfoDto");

            ExecuteNonQueryInner(
                () => _commandsDb.InsertAdditionalParam(additionalInfoDto.InfoKey, additionalInfoDto.InfoValue),
                executionContext);
        }

        public void Update(IExecutionContext executionContext, AdditionalInfo additionalInfoDto)
        {
            if (executionContext == null) throw new ArgumentNullException("executionContext");
            if (additionalInfoDto == null) throw new ArgumentNullException("additionalInfoDto");

            ExecuteNonQueryInner(
                () => _commandsDb.UpdateAdditionalParam(additionalInfoDto.InfoKey, additionalInfoDto.InfoValue),
                executionContext);
        }

        public void Delete(IExecutionContext executionContext, string key)
        {
            if (executionContext == null) throw new ArgumentNullException("executionContext");
            if (key == null) throw new ArgumentNullException("key");

            ExecuteNonQueryInner(() => _commandsDb.DeleteAdditionalParam(key), executionContext);
        }

        protected ILoader<AdditionalInfo> GetLoader(Func<IDbCommand> commandFunc)
        {
            return new Loader<AdditionalInfo>(commandFunc, AdditionalInfo.Load);
        }

        private readonly ICommandsDb _commandsDb;
    }
}
