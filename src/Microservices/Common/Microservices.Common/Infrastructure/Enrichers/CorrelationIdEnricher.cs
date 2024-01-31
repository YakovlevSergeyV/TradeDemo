namespace Microservices.Common.Infrastructure.Enrichers
{
    using System.Linq;
    using Microservices.Common.Infrastructure.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog.Core;
    using Serilog.Events;

    public class CorrelationIdEnricher : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "CorrelationId";
        private readonly string _headerKey;
        private IServiceCollection _services;

        public CorrelationIdEnricher(IServiceCollection services) : this(CorrelationIdOptions.Header, services)
        {
        }

        //public CorrelationIdEnricher(string headerKey) : this(headerKey, new HttpContextAccessor())
        //{
        //}

        private CorrelationIdEnricher(string headerKey, IServiceCollection services)
        {
            _headerKey = headerKey;
            _services = services;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContextAccessor = _services.BuildServiceProvider().GetService<IHttpContextAccessor>();
            if (httpContextAccessor.HttpContext == null) return;

            var correlationId = GetCorrelationId(httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(correlationId)) return;

            var correlationIdProperty = new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId));

            logEvent.AddPropertyIfAbsent(correlationIdProperty);
        }

        private string GetCorrelationId(HttpContext httpContext)
        {
            string correlationId = null;

            if (httpContext.Request.Headers.TryGetValue(_headerKey, out var values))
            {
                correlationId = values.FirstOrDefault();
            }

            return correlationId;
        }
    }
}
