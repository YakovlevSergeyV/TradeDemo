namespace TradesReader.Api
{
    using Microservices.Common;

    public class SettingsApplication : CommonSettingsApplication
    {
        public string ExchangeName { get; set; }
        public string Proxy { get; set; }
        public int RequestTimeoutSecond { get; set; }
    }
}
