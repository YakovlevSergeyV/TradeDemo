namespace TradesStorage.Api
{
    using Microservices.Common;

    public class SettingsApplication : CommonSettingsApplication
    {
        public string DatabaseDir { get; set; }
    }
}
