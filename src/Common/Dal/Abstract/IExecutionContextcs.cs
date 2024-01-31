
namespace GlobalSpace.Common.Dal.Abstract
{
    using System;
    using System.Data;

    /// <summary>
    /// Контекст доступа к базе данных
    /// </summary>
    public interface IExecutionContext
        : IDisposable
    {
        /// <summary>
        /// Открыть соединение
        /// </summary>
        void Open();

        /// <summary>
        /// Закрыть соединение
        /// </summary>
        void Close();

        /// <summary>
        /// Признак закрытого соединения
        /// </summary>
        bool ConnectionClosed { get; }

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Закоммитить транзакцию
        /// </summary>
        void Commit();

        /// <summary>
        /// Откатить транзакцию
        /// </summary>
        void Rollback();

        /// <summary>
        /// Выполнение команды без возврата результата
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        int ExecuteNonQuery(IDbCommand command);

        /// <summary>
        /// Выполненение команды с возвратом датаридера
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        IDataReader ExecuteReader(IDbCommand command);

        /// <summary>
        /// Выполнение команды с возвратом единственного результата
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        object ExecuteScalar(IDbCommand command);

        void PrepareCommand(IDbCommand command);
    }
}
