namespace Microservices.Common.WindowsService
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.WindowsServices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal class CustomWebHostService : WebHostService
    {
        private readonly string _name;
        private readonly ILogger<CustomWebHostService> _logger;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            _logger = host.Services
                .GetRequiredService<ILogger<CustomWebHostService>>();
            _name = nameof(CustomWebHostService);
        }

        protected override void OnStarting(string[] args)
        {
            _logger.LogDebug(_name, "OnStarting method called.");
            base.OnStarting(args);
        }

        protected override void OnStarted()
        {
            _logger.LogDebug(_name, "OnStarted method called.");
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            _logger.LogDebug(_name, "OnStopping method called.");
            base.OnStopping();
        }
    }
}
