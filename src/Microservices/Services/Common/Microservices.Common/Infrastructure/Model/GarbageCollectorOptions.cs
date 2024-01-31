namespace Microservices.Common.Infrastructure.Model
{
    using System;

    public class GarbageCollectorOptions
    {
        public GarbageCollectorOptions()
        {
            LastDateTime = DateTime.MinValue;
            HeartBeatCycleInMs = 10000;
        }

        public DateTime LastDateTime { get; set; }

        public int HeartBeatCycleInMs { get; set; }
    }
}
