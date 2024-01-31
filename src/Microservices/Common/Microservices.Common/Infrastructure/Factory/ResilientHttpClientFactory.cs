namespace Microservices.Common.Infrastructure.Factory
{
    using System;
    using System.Net.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Polly;
    using Resilience.Http;

    //public class ResilientHttpClientFactory : IResilientHttpClientFactory
    //{
    //    private readonly ILogger<ResilientHttpClient> _logger;
    //    private readonly int _retryCount;
    //    private readonly int _exceptionsAllowedBeforeBreaking;
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly IProviderHttpClient _providerHttpClient;

    //    public ResilientHttpClientFactory(
    //        IProviderHttpClient providerHttpClient,
    //        ILogger<ResilientHttpClient> logger, 
    //        IHttpContextAccessor httpContextAccessor,
    //        int exceptionsAllowedBeforeBreaking = 5, 
    //        int retryCount = 6)
    //    {
    //        _providerHttpClient = providerHttpClient;
    //        _logger = logger;
    //        _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
    //        _retryCount = retryCount;
    //        _httpContextAccessor = httpContextAccessor;
    //    }


    //    public ResilientHttpClient CreateResilientHttpClient()
    //        => new ResilientHttpClient(_providerHttpClient, (origin) => CreatePolicies(), _logger, _httpContextAccessor);

    //    private Policy[] CreatePolicies()
    //        => new Policy[]
    //        {
    //            Policy.Handle<HttpRequestException>()
    //            .WaitAndRetryAsync(
    //                // number of retries
    //                _retryCount,
    //                // exponential backofff
    //                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
    //                // on retry
    //                (exception, timeSpan, retryCount, context) =>
    //                {
    //                    var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
    //                        $"of {context.PolicyKey} " +
    //                        $"at {context.ExecutionKey}, " +
    //                        $"due to: {exception}.";
    //                    _logger.LogWarning(msg);
    //                    _logger.LogDebug(msg);
    //                }),
    //            Policy.Handle<HttpRequestException>()
    //            .CircuitBreakerAsync( 
    //               // number of exceptions before breaking circuit
    //               _exceptionsAllowedBeforeBreaking,
    //               // time circuit opened before retry
    //               TimeSpan.FromSeconds(30),
    //               (exception, duration) =>
    //               {
    //                    // on circuit opened
    //                    _logger.LogTrace("Circuit breaker opened");
    //               },
    //               () =>
    //               {
    //                    // on circuit closed
    //                    _logger.LogTrace("Circuit breaker reset");
    //               })
    //        };
    //}
}
