namespace TradesCoordinator.Infrastructure.Dto
{
    public class ExchangeInfo
    {
        public string Guid { get; set; }
        public string ExchangeName { get; set; }
        public int HeartBeatCycleInMs { get; set; }
        public long TimestampInitial { get; set; }
    }
}
