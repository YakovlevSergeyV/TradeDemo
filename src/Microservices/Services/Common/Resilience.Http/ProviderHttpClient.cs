namespace Resilience.Http
{
    using System;
    using System.Net;
    using System.Net.Http;

    public class ProviderHttpClient : IProviderHttpClient
    {
        private HttpSettings _settings;

        public ProviderHttpClient(HttpSettings settings)
        {
            _settings = settings;
        }

        public HttpClient GetHttpClient()
        {
            var clent = CreateClient();
            clent.Timeout = _settings.Timeout;
            return clent;
        }

        private HttpClient CreateClient()
        {
            // Это нужно было для версии 2.1, 2.0
             //AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);

            var handler = new HttpClientHandler();
            
            if (!string.IsNullOrEmpty(_settings.Proxy))
            {
                handler.Proxy = new WebProxy(_settings.Proxy) {UseDefaultCredentials = true};
            }
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            return new HttpClient(handler);
        }

        public HttpClient GetHttpClientTest()
        {
            if (string.IsNullOrEmpty(_settings.Proxy))
            {
                return new HttpClient();
            }

           // AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);

            string proxyUrl = _settings.Proxy;
            string proxyUsername = @"yakovlev";
            string proxyPassword = @"123Qazws";
            string proxyDomain = @"seaproject.ru";
            string authType = "NTLM";
           // string authType = "Negotiate";

            var credCache = new CredentialCache();
            var credentials = new NetworkCredential();
            credentials.UserName = proxyUsername;
            credentials.Password = proxyPassword;
          //  credentials.Domain = proxyDomain;

            credCache.Add(new Uri(proxyUrl), authType, credentials);

            var handler = new HttpClientHandler
            {
                UseProxy = true,
                Proxy = new WebProxy
                {
                    Address = new Uri(proxyUrl),
                //    Credentials = credCache
                }
            };

            return new HttpClient(handler);
        }
    }
}
