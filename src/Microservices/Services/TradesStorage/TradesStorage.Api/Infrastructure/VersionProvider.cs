namespace TradesStorage.Api.Infrastructure
{
    using System;
    using global::Infrastructure.Common.Providers;

    public class VersionProvider : IVersionProvider
    {
        public Version GetVersion()
        {
            return new Version();
        }
    }
}
