namespace GlobalSpace.Common.Dal.Abstract
{
    using System;
    using System.Data;

    public abstract class ExecutionContext
    : MarshalByRefObject, IExecutionContext
    {
        private TimeSpan _timeout;

        protected int RealTimeout;

        protected ExecutionContext(TimeSpan timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Таймаут операции
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                CheckIfDisposed();

                return _timeout;
            }
            set
            {
                CheckIfDisposed();

                _timeout = value;
                RealTimeout = (int)_timeout.TotalMilliseconds;
            }
        }

        public abstract void Dispose();

        public abstract void Open();
        public abstract void Close();
        public abstract void BeginTransaction();
        public abstract void Commit();
        public abstract void Rollback();
        public abstract bool ConnectionClosed { get; }

        protected abstract IDbCommand CreateCommand(string script);

        /// <summary>
        /// Выполненение команды с возвратом датаридера
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        public abstract IDataReader ExecuteReader(IDbCommand command);

        /// <summary>
        /// Выполнение команды с возвратом единственного результата
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        public abstract object ExecuteScalar(IDbCommand command);

        /// <summary>
        /// Выполнение команды без возврата результата
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        public abstract int ExecuteNonQuery(IDbCommand command);

        /// <summary>
        /// Выполнение команды без возврата результата
        /// </summary>
        /// <param name="commandFunc">Функция, возвращающая команду</param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(Func<IDbCommand> commandFunc)
        {
            CheckIfDisposed();
            if (commandFunc == null) throw new ArgumentNullException("commandFunc");

            using (var command = commandFunc.Invoke())
            {
                return ExecuteNonQuery(command);
            }
        }
        
        protected virtual void Prepare(IDbCommand command)
        {
            CheckIfDisposed();
            if (command == null) throw new ArgumentNullException("command");

            OpenConnectionIfClosed();
            PrepareCommand(command);
        }

        public abstract void PrepareCommand(IDbCommand command);
        protected abstract void OpenConnectionIfClosed();
        protected abstract void CheckIfDisposed();
    }
}
