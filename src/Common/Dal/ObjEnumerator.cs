namespace GlobalSpace.Common.Dal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using GlobalSpace.Common.Dal.Abstract;

    /// <summary>
    /// Енумератор объектов
    /// </summary>
    /// <typeparam name="T">Тип перечисляемых объектов</typeparam>
    public sealed class ObjEnumerator<T>
        : IEnumerator<T>
        where T : class
    {
        private readonly IExecutionContext _executionContext;
        private readonly ILoader<T> _loader;

        private bool _disposed;
        private IDbCommand _dbCommand;
        private IReaderWrapper _readerWrapper;
        private IDataReader _dataReader;
        private T _current;

        public ObjEnumerator(IExecutionContext executionContext, ILoader<T> loader)
        {
            if (executionContext == null) throw new ArgumentNullException("executionContext");
            if (loader == null) throw new ArgumentNullException("loader");

            _executionContext = executionContext;
            _loader = loader;

            Reset();
        }

        ~ObjEnumerator()
        {
            Dispose();
        }

        public T Current
        {
            get
            {
                CheckIfDisposed();

                if (_current == null)
                    throw new InvalidOperationException("Текущий элемент отсутствует.");

                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                CheckIfDisposed();
                return Current;
            }
        }

        public bool MoveNext()
        {
            CheckIfDisposed();

            if (_dataReader.Read())
            {
                _current = _loader.MapFunc.Invoke(_readerWrapper);
                return true;
            }

            _current = null;
            return false;
        }

        public void Reset()
        {
            CheckIfDisposed();

            if (_dataReader != null)
                _dataReader.Dispose();

            if (_dbCommand != null)
                _dbCommand.Dispose();

            _dbCommand = _loader.DbCommand.Invoke();
            _dataReader = _executionContext.ExecuteReader(_dbCommand);
            _readerWrapper = ReaderWrapperFactory.Current.Create(_dataReader);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_dataReader != null)
                _dataReader.Dispose();

            if (_dbCommand != null)
                _dbCommand.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void CheckIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("ObjEnumerator");
        }
    }
}
