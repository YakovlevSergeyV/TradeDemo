namespace TradesReader.Api.Request
{
    using global::Infrastructure.Common.Convert;
    using Microservices.Common.Infrastructure.Request;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Resilience.Http;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TradesReader.Api.Dto;
    using TradesReader.Api.Extensions;

    internal class Request :  RequestAbstract, IRequest
    {
        private readonly UrlsConfig _urls;

        public Request
        (
            IOptionsSnapshot<UrlsConfig> config,
            IProviderHttpClient providerHttpClient,
            Func<IProviderHttpClient, IHttpClient> httpClientFactory,
            IHttpContextAccessor httpContextAccessor
        ) : base(providerHttpClient, httpClientFactory, httpContextAccessor)
        {
            _urls = config.Value;
        }

        #region Markets

        public async Task<IEnumerable<Trade>> GetMarketTradesAsync(string exchange, ParametersTrade parameters)
        {
            //var token = await GetToken(urls.Identity, AuthorizationInfo.ScopeMarkets);
            var token = (string)null;

            var timestampMin = ConvertDateTime.DateToUnixTimeStampMilliseconds(parameters.TimeStart);
            var timestampMax = ConvertDateTime.DateToUnixTimeStampMilliseconds(parameters.TimeEnd);
            var url = _urls.Markets + _urls.GetMarketTrades(exchange, parameters.Symbol, timestampMin, timestampMax);

            var result = await httpClient.GetStringAsync(url, token);

            return string.IsNullOrEmpty(result)
                ? new List<Trade>()
                : JsonConvert.DeserializeObject<IEnumerable<Trade>>(result);
        }

        #endregion

        #region TradesStorage

        public async Task<TradeInfo> GetTradeInfoAsync(string exchange, string symbol)
        {
            var url = _urls.TradeStorage + _urls.GetTradeInfo(exchange, symbol);
            var result = await httpClient.GetStringAsync(url);

            return string.IsNullOrEmpty(result) ? null : JsonConvert.DeserializeObject<TradeInfo>(result);
        }

        public async Task TradeUpdateTimestampAsync(string exchange, string symbol, long timestamp)
        {
            var url = _urls.TradeStorage + _urls.GetUpdateTimestamp(exchange, symbol, timestamp);

            var result = await httpClient.PostAsync(url, new List<Trade>());
            EnsureSuccessStatusCode(result);
        }

        public async Task TradeInsertAsync(string exchange, string symbol, long timestamp, IEnumerable<Trade> trades)
        {
            var url = _urls.TradeStorage + _urls.GetInsert(exchange, symbol, timestamp);

            var result = await httpClient.PostAsync(url, trades);
            EnsureSuccessStatusCode(result);
        }

        #endregion

        #region TradesCoordinator

        public async Task<SymbolInfo> GetTradeCoordinatorSymbolAsync(string exchange, string symbol)
        {
            var url = _urls.TradeCoordinator + _urls.GetSymbol(exchange, symbol);

            var result = await httpClient.GetStringAsync(url);
            return string.IsNullOrEmpty(result)
                ? null
                : JsonConvert.DeserializeObject<SymbolInfo>(result);
        }

        public async Task TradeCoordinatorUpdateSymbolAsync(SymbolInfo symbolInfo)
        {
            var url = _urls.TradeCoordinator + _urls.GetUpdateSymbol();

            var result = await httpClient.PutAsync<SymbolInfo>(url, symbolInfo);
            EnsureSuccessStatusCode(result);
        }

        public async Task<SymbolNext> GetTradeNextSymbolsAsync(ServiceInfo serviceInfo)
        {
            var url = _urls.TradeCoordinator + _urls.GetNextsymbols();
            var result = await httpClient.PostAsync<ServiceInfo>(url, serviceInfo);
            EnsureSuccessStatusCode(result);

            return await JsonAsync<SymbolNext>(result);
        }

        public async Task<ExchangeInfo> GetTradeExchangeInfoAsync(string exchange)
        {
            var url = _urls.TradeCoordinator + _urls.GetExchange(exchange);

            var result = await httpClient.GetStringAsync(url);
            return string.IsNullOrEmpty(result)
                ? null
                : JsonConvert.DeserializeObject<ExchangeInfo>(result);
        }

        public async Task<bool> GetTradeServiceStartedAsync(ServiceInfo serviceInfo)
        {
            var url = _urls.TradeCoordinator + _urls.GetServiceStarted();

            var result = await httpClient.PostAsync<ServiceInfo>(url, serviceInfo);
            EnsureSuccessStatusCode(result);

            return await JsonAsync<bool>(result);
        }

        public async Task TradePerformanceNotSuccessfulAsync(PerformanceNotSuccessful performance)
        {
            var url = _urls.TradeCoordinator + _urls.GetPerformanceNotSuccessful();

            var result = await httpClient.PostAsync<PerformanceNotSuccessful>(url, performance);
            EnsureSuccessStatusCode(result);
        }

        public async Task TradePerformanceSuccessfulAsync(PerformanceSuccessful performance)
        {
            var url = _urls.TradeCoordinator + _urls.GetPerformanceSuccessful();

            var result = await httpClient.PostAsync<PerformanceSuccessful>(url, performance);
            EnsureSuccessStatusCode(result);
        }

        public async Task TradeRegisterServiceAsync(ServiceInfo serviceInfo)
        {
            var url = _urls.TradeCoordinator + _urls.GetRegisterService();

            var result = await httpClient.PostAsync<ServiceInfo>(url, serviceInfo);
            EnsureSuccessStatusCode(result);
        }

        #endregion
    }
}
