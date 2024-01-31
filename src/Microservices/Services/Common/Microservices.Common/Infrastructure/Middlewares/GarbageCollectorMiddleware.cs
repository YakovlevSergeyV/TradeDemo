namespace Microservices.Common.Infrastructure.Middlewares
{
    using System;
    using System.Threading.Tasks;
    using Microservices.Common.Infrastructure.Model;
    using Microsoft.AspNetCore.Http;

    public class GarbageCollectorMiddleware
    {
        private readonly RequestDelegate next;
        private GarbageCollectorOptions options;

        public GarbageCollectorMiddleware(RequestDelegate next, GarbageCollectorOptions options)
        {
            this.next = next;
            this.options = options;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var end = DateTime.UtcNow;
            var start = options.LastDateTime.Ticks / TimeSpan.TicksPerMillisecond;
            var difference = (end.Ticks / TimeSpan.TicksPerMillisecond - start);

            if (difference >= options.HeartBeatCycleInMs)
            {
                GC.Collect(2, GCCollectionMode.Forced, false);
                options.LastDateTime = end;
            }
            await next.Invoke(httpContext);
        }
    }
}
