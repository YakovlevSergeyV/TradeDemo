namespace TradesCoordinator.Api.Infrastructure.UpgradeDb
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using TradesCoordinator.Infrastructure.UpgradeDb;

    public static class DatabaseUpgradeExtensions
    {
        public static IApplicationBuilder UpgradeDb(this IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetRequiredService<IOptions<SettingsApplication>>().Value;

            var databaseUpgradeFactory = app.ApplicationServices
                .GetService<Func<string, IDatabaseUpgrade>>();

            databaseUpgradeFactory(settings.DatabaseDir).Run();

            return app;
        }
    }
}
