namespace GlobalSpace.Common.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Guardly;

    /// <summary>
    /// Helper-класс, позволяющий более удобно читать данные из IDataReader
    /// </summary>
    public sealed class ReaderWrapper
        : IReaderWrapper
    {
        private readonly IDataReader _dataReader;
        private Lazy<IDictionary<string, int>> _columnList;

        public ReaderWrapper(IDataReader dataReader)
        {
            Guard.Argument(() => dataReader, Is.NotNull);

            _dataReader = dataReader;
            _columnList = new Lazy<IDictionary<string, int>>(GetColumnList, true);
        }

        public IDictionary<string, int> ColumnList { get { return _columnList.Value; } }

        public bool Read()
        {
            return _dataReader.Read();
        }

        public bool NextResult()
        {
            if (_dataReader.NextResult())
            {
                _columnList = new Lazy<IDictionary<string, int>>(GetColumnList, true);
                return true;
            }

            return false;
        }

        public object Read(string columnName)
        {
            columnName = columnName.ToLower();
            if (!ColumnList.ContainsKey(columnName))
                return null;

            var type = _dataReader.GetFieldType(ColumnList[columnName]);
            var result = _dataReader.GetValue(ColumnList[columnName]);
            if (result == DBNull.Value)
                return Convert.ChangeType(null, type);

            return Convert.ChangeType(result, type);
        }

        public T Read<T>(string columnName)
        {
            columnName = columnName.ToLower();
            if (!ColumnList.ContainsKey(columnName))
                return default(T);

            var result = _dataReader.GetValue(ColumnList[columnName]);
            if (result == DBNull.Value)
                return default(T);

            return (T)result;
        }

        public bool Contains(string columnName)
        {
            columnName = columnName.ToLower();

            return ColumnList.ContainsKey(columnName);
        }

        private IDictionary<string, int> GetColumnList()
        {
            var columnList = new Dictionary<string, int>();
            for (int i = 0; i < _dataReader.FieldCount; i++)
            {
                var name = _dataReader.GetName(i);
                columnList.Add(name.ToLower(), i);
            }

            return columnList;
        }
    }
}
