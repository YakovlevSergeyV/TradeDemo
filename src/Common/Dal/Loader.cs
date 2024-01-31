namespace GlobalSpace.Common.Dal
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Guardly;

    /// <summary>
    /// Класс, позволяющий разнести по времени создание команды на операцию выборки и вызов мэппинга полученных данных на объекты
    /// </summary>
    /// <typeparam name="T">Любой тип, являющийся классом, который должен получиться в результате этой выборки</typeparam>
    public class Loader<T>
        : ILoader<T> where T : class
    {
        public Loader(Func<IDbCommand> dbCommand, Func<IReaderWrapper, T> mapFunc)
        {
            Guard.Argument(() => dbCommand, Is.NotNull);
            Guard.Argument(() => mapFunc, Is.NotNull);

            DbCommand = dbCommand;
            MapFunc = mapFunc;
        }

        public Func<IDbCommand> DbCommand { get; private set; }
        public Func<IReaderWrapper, T> MapFunc { get; private set; }
    }
}