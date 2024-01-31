namespace GlobalSpace.Common.Dal.Abstract
{
    using System;
    using System.Data;

    public interface ILoader<out T>
        where T : class
    {
        Func<IDbCommand> DbCommand { get; }
        Func<IReaderWrapper, T> MapFunc { get; }
    }
}
