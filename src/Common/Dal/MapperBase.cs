namespace GlobalSpace.Common.Dal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using GlobalSpace.Common.Dal.Abstract;

    /// <summary>
    /// Абстрактный базовый мэппер.
    /// </summary>
    public abstract class MapperBase
    {
        /// <summary>
        /// Читает из базы несколько наборов данных и мэппит их на разные типы. Ограничение - каждый набор должен соответствовать уникальному типу
        /// </summary>
        /// <param name="commandFunc">Функция, которая вернёт набор ридеров</param>
        /// <param name="mapperFunctions">Список функций маппинга</param>
        /// <param name="executionContext">Контекст исполнения</param>
        /// <returns></returns>
        protected virtual IDictionary<Type, IList> ListAllInner(Func<IDbCommand> commandFunc, IDictionary<Type, Func<IDataReader, IEnumerable>> mapperFunctions, IExecutionContext executionContext)
        {
            Debug.Assert(commandFunc != null);
            Debug.Assert(mapperFunctions != null);
            Debug.Assert(executionContext != null);

            var result = new Dictionary<Type, IList>();
            using (var dbCommand = commandFunc.Invoke())
            {
                using (var dataReader = executionContext.ExecuteReader(dbCommand))
                {
                    foreach (var mapperFunction in mapperFunctions)
                    {
                        if (dataReader == null)
                            break;

                        var list = (IList)Activator.CreateInstance((typeof(List<>).MakeGenericType(mapperFunction.Key)));
                        var enumerable = mapperFunction.Value.Invoke(dataReader);

                        foreach (var en in enumerable)
                        {
                            list.Add(en);
                        }

                        result.Add(mapperFunction.Key, list);

                        if (!dataReader.NextResult())
                            break;
                    }
                }
            }

            return result;
        }
    }

    public abstract class MapperBase<T> : MapperBase where T : class
    {
        protected virtual T FindInner(ILoader<T> loader, IExecutionContext executionContext)
        {
            using (IDbCommand command = loader.DbCommand())
            {
                using (IDataReader dataReader = executionContext.ExecuteReader(command))
                {
                    if (dataReader.Read())
                    {
                        ReaderWrapper readerWrapper = new ReaderWrapper(dataReader);
                        return loader.MapFunc((IReaderWrapper)readerWrapper);
                    }
                }
            }
            return default(T);
        }

        protected virtual void ExecuteNonQueryInner(Func<IDbCommand> commandFunc, IExecutionContext executionContext)
        {
            using (IDbCommand command = commandFunc())
                executionContext.ExecuteNonQuery(command);
        }

        protected virtual object ExecuteScalar(Func<IDbCommand> commandFunc, IExecutionContext executionContext)
        {
            using (IDbCommand command = commandFunc())
                return executionContext.ExecuteScalar(command);
        }

        protected IEnumerable<T> EnumAllInner(ILoader<T> loader, IExecutionContext executionContext)
        {
            if (loader == null)
                throw new ArgumentNullException($"loader");
            if (executionContext == null)
                throw new ArgumentNullException($"executionContext");
            return (IEnumerable<T>)new ObjEnumerable<T>(executionContext, loader);
        }
    }
}
