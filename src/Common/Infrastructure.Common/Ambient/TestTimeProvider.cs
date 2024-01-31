namespace Infrastructure.Common.Ambient
{
    using System;

    public class TestTimeProvider : TimeProvider
    {
        public TestTimeProvider(DateTime now)
        {
            Now = now;
            UtcNow = DateTime.UtcNow;
        }

        public TestTimeProvider(DateTime now, DateTime utcNow)
        {
            Now = now;
            UtcNow = utcNow;
        }

        public override DateTime UtcNow { get; }

        public override DateTime Now { get; }
    }
}
