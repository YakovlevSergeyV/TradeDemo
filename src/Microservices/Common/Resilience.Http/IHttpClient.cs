namespace Resilience.Http
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IHttpClient
    {
        Task<string> GetTokenAsync(string identityUrl, string clientId, string clientSecret, string scope);

        Task<string> GetStringAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearer");

        Task<string> GetStringContentAsync<T>(string uri, T item, string authorizationToken = null,
            string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PostCompressAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null,
            string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> DeleteAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");
    }
}
