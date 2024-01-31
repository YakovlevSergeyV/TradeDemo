namespace TradesStorage.Api.IntegrationEvents.Events
{
    using EventBus.Events;

    public class TradesActualInsertedIntegrationEvent : IntegrationEvent
    {
        public TradesActualInsertedIntegrationEvent(
            string exchangeName,
            string currencyPairName, 
            long timestampMin,
            long timestampMax)
        {
            ExchangeName = exchangeName;
            CurrencyPairName = currencyPairName;
            TimestampMin = timestampMin;
            TimestampMax = timestampMax;
        }

        public string ExchangeName { get; set; }
        public string CurrencyPairName { get; set; }
        public long TimestampMin { get; set; }
        public long TimestampMax { get; set; }
    }
}
