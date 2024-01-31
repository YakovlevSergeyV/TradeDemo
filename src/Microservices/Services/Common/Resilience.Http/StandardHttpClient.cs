namespace Resilience.Http
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class StandardHttpClient : HttpClientAbstract, IHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<StandardHttpClient> _logger;

        public StandardHttpClient(
            IProviderHttpClient providerHttpClient,
            ILogger<StandardHttpClient> logger,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _client = providerHttpClient.GetHttpClient();
            _logger = logger;
        }

        public async Task<string> GetTokenAsync(string identityUrl, string clientId, string clientSecret, string scope)
        {
            //Временное решение, для отключения HTTPS для identityServer
            //var request = new DiscoveryDocumentRequest { Address = identityUrl };
            //request.Policy.RequireHttps = false;
            //var disco = await client.GetDiscoveryDocumentAsync(request);
            var disco = await _client.GetDiscoveryDocumentAsync(identityUrl);
            if (disco.IsError)
            {
                throw new AuthenticationException(disco.Error);
            }

            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = scope
            });

            if (tokenResponse.IsError)
            {
                throw new AuthenticationException(tokenResponse.Error);
            }

            return tokenResponse.AccessToken;
        }

        public async Task<string> GetStringAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            SetAuthorizationHeader(requestMessage);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            var response = await _client.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetStringContentAsync<T>(string uri, T item, string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            requestMessage.Content = httpContent;

            SetAuthorizationHeader(requestMessage);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            var response = await _client.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            return await DoAsync(_client, HttpMethod.Post, uri, item, authorizationToken, requestId,
                authorizationMethod);
        }

        public async Task<HttpResponseMessage> PostCompressAsync<T>(string uri, T item,
            string authorizationToken = null, string requestId = null,
            string authorizationMethod = "Bearer")
        {
            return await DoAsync(_client, HttpMethod.Post, uri, item, authorizationToken, requestId,
                authorizationMethod,
                true);
        }

        public async Task<HttpResponseMessage> DeleteAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null,
            string authorizationMethod = "Bearer")
        {
            return await DoAsync(_client, HttpMethod.Delete, uri, item, authorizationToken, requestId,
                authorizationMethod);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            return await DoAsync(_client, HttpMethod.Put, uri, item, authorizationToken, requestId,
                authorizationMethod);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            SetAuthorizationHeader(requestMessage);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            return await _client.SendAsync(requestMessage);
        }
    }
}
