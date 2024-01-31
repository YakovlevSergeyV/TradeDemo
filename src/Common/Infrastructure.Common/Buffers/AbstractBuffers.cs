namespace Infrastructure.Common.Buffers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class AbstractBuffers<T>
    {
        private readonly ConcurrentQueue<IEnumerable<T>> _buffers;

        protected AbstractBuffers()
        {
            _buffers = new ConcurrentQueue<IEnumerable<T>>();
        }

        public void Add(T item)
        {
            _buffers.Enqueue(new []{item});
        }

        public void Add(IEnumerable<T> items)
        {
            _buffers.Enqueue(items);
        }

        public IEnumerable<T> Get()
        {
            var items = new List<T>();
            do
            {
                _buffers.TryDequeue(out var dto);
                if (dto != null) items.AddRange(dto);
            } while (_buffers.Any());

            return items;
        }
    }
}
