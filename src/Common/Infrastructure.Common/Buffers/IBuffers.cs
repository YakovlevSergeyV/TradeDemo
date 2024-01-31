
namespace Infrastructure.Common.Buffers
{
    using System.Collections.Generic;

    public interface IBuffers<T>
    {
        void Add(T item);
        void Add(IEnumerable<T> item);
        IEnumerable<T> Get();
    }
}
