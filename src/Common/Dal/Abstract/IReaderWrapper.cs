namespace GlobalSpace.Common.Dal.Abstract
{
    using System.Collections.Generic;

    public interface IReaderWrapper
    {
        IDictionary<string, int> ColumnList { get; }
        bool Read();
        bool NextResult();
        object Read(string columnName);
        T Read<T>(string columnName);
        bool Contains(string columnName);
    }
}
