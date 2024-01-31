
namespace GlobalSpace.Common.Dal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GlobalSpace.Common.Dal.Abstract;

    /// <summary>
    /// Базовый енумератор объектов, получаемых из данных
    /// </summary>
    /// <typeparam name="T">Тип перечисляемых объектов</typeparam>
    public class ObjEnumerable<T>
        : IEnumerable<T>
        where T : class
    {
        private readonly IExecutionContext _executionContext;
        private readonly ILoader<T> _loader;

        public ObjEnumerable(IExecutionContext executionContext, ILoader<T> loader)
        {
            if (executionContext == null) throw new ArgumentNullException("executionContext");
            if (loader == null) throw new ArgumentNullException("loader");

            _executionContext = executionContext;
            _loader = loader;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ObjEnumerator<T>(_executionContext, _loader);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
