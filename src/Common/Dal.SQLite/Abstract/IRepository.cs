namespace GlobalSpace.Common.Dal.SQLite.Abstract
{
    using System;
    using GlobalSpace.Common.Dal.Abstract;

    public interface IRepository
    {
        void Upgrade(IExecutionContext executionContext);

        T GetAdditionalParam<T>(string key, T defaultValue, IExecutionContext executionContext);
        void SetAdditionalParam<T>(string key, T value, IExecutionContext executionContext);
        Version GetSchemaVersion(IExecutionContext executionContext);

        #region Pragma

        void JournalModeMemory(IExecutionContext executionContext);
        void SynchronousOff(IExecutionContext executionContext);
        void SynchronousOn(IExecutionContext executionContext);

        #endregion
    }
}
