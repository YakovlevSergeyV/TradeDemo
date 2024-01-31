namespace TradesReader.Api.Dto
{
    internal class PerformanceNotSuccessful : Service
    {
        public string CurrencyPairName { get; set; }
        public string Exception { get; set; }
    }
}
