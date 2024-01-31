
namespace Resilience.Http
{
    using System;

    public class HttpSettings
    {
        public HttpSettings()
        {
            Timeout = new TimeSpan(0, 0, 0, 10);
            Proxy = string.Empty;
        }

        public TimeSpan Timeout { get; set; }

        public string Proxy { get; set; }
    }
}
