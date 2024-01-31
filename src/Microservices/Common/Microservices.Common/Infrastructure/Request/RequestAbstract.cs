namespace Microservices.Common.Infrastructure.Request
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using global::Infrastructure.Common.Authorization;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Resilience.Http;

    public abstract class RequestAbstract
    {
        protected readonly IHttpClient httpClient;
        protected readonly IHttpContextAccessor httpContextAccessor;

        protected RequestAbstract
        (
            IProviderHttpClient providerHttpClient,
            Func<IProviderHttpClient, IHttpClient> httpClientFactory,
            IHttpContextAccessor httpContextAccessor
        )
        {
            httpClient = httpClientFactory.Invoke(providerHttpClient);
            this.httpContextAccessor = httpContextAccessor;
        }

        protected async Task<string> GetToken(string identityUrls, string scope)
        {
            return await httpClient.GetTokenAsync(identityUrls, AuthorizationInfo.ClientId, AuthorizationInfo.Secret,
                scope);
        }

        protected IEnumerable<string> GetHosts(string value, string exchange)
        {
            var hosts = new List<string>();
            foreach (var row in value.Split(";"))
            {
                var values = row.Split("|");
                if (values.Length > 0 && values[0] == exchange)
                {
                    hosts.Add(values[1]);
                }
            }

            return hosts;
        }

        protected void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw new Exception(response.ToString());
            }
        }

        protected async Task<T> JsonAsync<T>(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();
            return (string.IsNullOrEmpty(result)) ? default(T) : JsonConvert.DeserializeObject<T>(result);
        }

        protected async Task<string> GetUserTokenAsync()
        {
            return null;
            //var context = httpContextAccessor?.HttpContext;
            //if (context == null) return null;
            //return await context.GetTokenAsync("access_token");
        }
    }
}
