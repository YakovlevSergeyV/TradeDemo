namespace GlobalSpace.Common.Dal.SQLite
{
    using System;
    using System.Data;
    using System.Reflection;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Guardly;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.Logging;

    public sealed class SqLiteExecutionContext : ExecutionContext
    {
        private SqliteConnection _connection;
        private SqliteTransaction _transaction;
        private bool _isConnectionClosed;
        private bool _disposed;
        private readonly ILogger<SqLiteExecutionContext> _logger;


        public SqLiteExecutionContext(
            IOptionsSqLite optionsSqLite
            ,ILogger<SqLiteExecutionContext> logger)
            : base(optionsSqLite.SqliteTimeout)
        {
            Guard.Argument(() => optionsSqLite, Is.NotNull);

            _logger = logger;
            _connection = new SqliteConnection(optionsSqLite.GetConnectionString());
            _transaction = null;
            _isConnectionClosed = true;
            _disposed = false;
        }

        ~SqLiteExecutionContext()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (_disposed)
                return;

            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }

            if (_connection != null)
            {
                try
                {
                    _connection.Close();
                    // SqliteConnection.ClearPool(_connection);
                }
                catch (Exception exception)
                {
                    _logger?.LogError(exception,
                            "Ошибка закрытия соединения при удалении его в методе Dispose().");
                }

                _connection.Dispose();
                _connection = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// Открыть соединение
        /// </summary>
        public override void Open()
        {
            CheckIfDisposed();
            _connection.Open();
            _connection.DefaultTimeout = 600;
            _isConnectionClosed = false;
        }

        /// <summary>
        /// Закрыть соединение
        /// </summary>
        public override void Close()
        {
            CheckIfDisposed();
            _connection.Close();
            _isConnectionClosed = true;
        }

        public override bool ConnectionClosed => _isConnectionClosed;

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        public override void BeginTransaction()
        {
            CheckIfDisposed();
            _transaction = _connection.BeginTransaction();
        }

        public override void Commit()
        {
            CheckIfDisposed();
            if (_transaction == null)
                throw new InvalidOperationException("Нет открытой транзакции.");

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        public override void Rollback()
        {
            CheckIfDisposed();
            if (_transaction == null)
                return;

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        /// <summary>
        /// Выполнение команды с возвратом единственного результата
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        public override object ExecuteScalar(IDbCommand command)
        {
            Prepare(command);

            try
            {
                return command.ExecuteScalar();
            }
            catch (SqliteException exception)
            {
                _logger?.LogError(exception, "Ошибка обращения к SQLite.");
                throw;
            }
        }

        public override int ExecuteNonQuery(IDbCommand command)
        {
            Prepare(command);

            try
            {
                return command.ExecuteNonQuery();
            }
            catch (SqliteException exception)
            {
                _logger?.LogError(exception, "Ошибка обращения к SQLite.");
                throw;
            }
        }

        public override IDataReader ExecuteReader(IDbCommand command)
        {
            Prepare(command);

            try
            {
                return command.ExecuteReader();
            }
            catch (SqliteException exception)
            {
                _logger?.LogError(exception, "Ошибка обращения к SQLite.");
                throw;
            }
        }
        
        public override void PrepareCommand(IDbCommand command)
        {
            command.Connection = _connection;
            command.Transaction = _transaction;
            command.CommandTimeout = RealTimeout;
        }

        protected override void OpenConnectionIfClosed()
        {
            if (_connection.State != ConnectionState.Open)
                Open();
        }

        protected override void CheckIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(MethodBase.GetCurrentMethod().GetType().FullName);
        }

        protected override IDbCommand CreateCommand(string script)
        {
            return new SqliteCommand(script);
        }
    }
}
