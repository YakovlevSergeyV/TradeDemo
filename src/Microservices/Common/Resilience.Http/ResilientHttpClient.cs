namespace Resilience.Http
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
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
    using Polly;
    using Polly.Wrap;

    /// <summary>
    /// HttpClient wrapper that integrates Retry and Circuit
    /// breaker policies when invoking HTTP services. 
    /// Based on Polly library: https://github.com/App-vNext/Polly
    /// </summary>
    public class ResilientHttpClient : HttpClientAbstract, IHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly Func<string, IEnumerable<IAsyncPolicy>> _policyCreator;
        private ConcurrentDictionary<string, AsyncPolicyWrap> _policyWrappers;

        public ResilientHttpClient(
            IProviderHttpClient providerHttpClient,
            Func<string, IEnumerable<IAsyncPolicy>> policyCreator,
            ILogger<ResilientHttpClient> logger,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _client = providerHttpClient.GetHttpClient();
            _logger = logger;
            _policyCreator = policyCreator;
            _policyWrappers = new ConcurrentDictionary<string, AsyncPolicyWrap>();
        }

        public async Task<string> GetTokenAsync(string identityUrl, string clientId, string clientSecret, string scope)
        {
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

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> PostCompressAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            return DoAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod, true);
        }

        public Task<HttpResponseMessage> DeleteAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null,
            string authorizationMethod = "Bearer")
        {
            return DoAsync(HttpMethod.Delete, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
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
            });
        }

        public Task<string> GetStringAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
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
            });
        }

        public Task<string> GetStringContentAsync<T>(string uri, T item, string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
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
            });
        }

        private Task<HttpResponseMessage> DoAsync<T>(HttpMethod method, string uri, T item,
            string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer",
            bool compressed = false)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put && method != HttpMethod.Delete)
            {
                throw new ArgumentException("Value must be either post, put, delete.", nameof(method));
            }

            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () => await DoAsync(_client, method, uri, item,
                authorizationToken, requestId, authorizationMethod, compressed));
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out AsyncPolicyWrap policyWrap))
            {
                 policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            // Executes the action applying all 
            // the policies defined in the wrapper
            return await policyWrap.ExecuteAsync(action);
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }
    }
}
