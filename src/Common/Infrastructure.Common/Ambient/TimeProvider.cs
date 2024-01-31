namespace Infrastructure.Common.Ambient
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public abstract class TimeProvider
    {
        private static TimeProvider _current;

        static TimeProvider()
        {
            ResetToDefault();
        }

        public static TimeProvider Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
            }
        }

        public abstract DateTime UtcNow { get; }
        public abstract DateTime Now { get; }

        public static void ResetToDefault()
        {
            _current = new DefaultTimeProvider();
        }

        internal class DefaultTimeProvider
            : TimeProvider
        {
            public override DateTime UtcNow => DateTime.UtcNow;

            public override DateTime Now => DateTime.Now;
        }
    }
}
