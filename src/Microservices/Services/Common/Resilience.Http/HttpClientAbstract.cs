namespace Resilience.Http
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public abstract class HttpClientAbstract
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected HttpClientAbstract(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<HttpResponseMessage> DoAsync<T>(HttpClient client, HttpMethod method, string uri, T item,
            string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer",
            bool compressed = false)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put && method != HttpMethod.Delete)
            {
                throw new ArgumentException("Value must be either post, put, delete.", nameof(method));
            }

            var requestMessage = new HttpRequestMessage(method, uri);

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

            return compressed
                ? await SendCompressedAsync(requestMessage, item, client)
                : await SendAsync(requestMessage, item, client);
        }

        protected async Task<HttpResponseMessage> SendAsync<T>(HttpRequestMessage requestMessage, T content,
            HttpClient httpClient)
        {
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            requestMessage.Content = httpContent;
            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }

        protected async Task<HttpResponseMessage> SendCompressedAsync<T>(HttpRequestMessage requestMessage, T content,
            HttpClient httpClient)
        {
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            requestMessage.Content = new CompressedContent(httpContent, "gzip");
            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
            return response;
        }


        protected void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            if (_httpContextAccessor.HttpContext == null) return;
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() {authorizationHeader});
            }
        }
    }
}
