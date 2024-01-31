namespace GlobalSpace.Common.Dal
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal.Abstract;

    public abstract class ReaderWrapperFactory
    {
        private static volatile object _syncRoot = new object();

        private static ReaderWrapperFactory _current;

        public static ReaderWrapperFactory Current
        {
            get
            {
                if (_current == null)
                {
                    lock (_syncRoot)
                    {
                        if (_current == null)
                        {
                            _current = new DefaultReaderWrapperFactory();
                        }
                    }
                }

                return _current;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _current = value;
            }
        }

        public abstract IReaderWrapper Create(IDataReader dataReader);

        private class DefaultReaderWrapperFactory
            : ReaderWrapperFactory
        {
            public override IReaderWrapper Create(IDataReader dataReader)
            {
                return new ReaderWrapper(dataReader);
            }
        }
    }
}
