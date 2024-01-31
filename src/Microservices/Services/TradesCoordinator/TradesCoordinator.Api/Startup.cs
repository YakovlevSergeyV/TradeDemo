namespace TradesCoordinator.Api
{
    using System;
    using System.Reflection;
    using Autofac;
    using EventBus.Abstractions;
    using global::Infrastructure.Common;
    using GlobalSpace.Common.Dal.SQLite.Model;
    using Microservices.Common;
    using Microservices.Logging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SQLitePCL;
    using TradesCoordinator.Api.Infrastructure.AutofacModules;
    using TradesCoordinator.Api.Infrastructure.UpgradeDb;
    using TradesCoordinator.Api.IntegrationEvents.EventHandling;
    using TradesCoordinator.Api.IntegrationEvents.Events;
    using TradesCoordinator.Infrastructure.EntityContext;
    using TradesCoordinator.Infrastructure.Synchronization;
    using TradesCoordinator.Infrastructure.UpgradeDb;

    public class Startup : CommonStartup<SettingsApplication>
    {
        private static readonly string _applicationName;

        static Startup()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var name = currentAssembly.GetName().Name;
            _applicationName = name.Replace(".Api", "");
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment)
        {
        }

        protected override string ApplicationName => _applicationName;

        protected override void RegisterServicesAdditional(IServiceCollection services)
        {
            services.AddDbContext<CoordinatorContext>(options =>
            {
                var path = Configuration["DatabaseDir"];
                var optionsSqLite = new OptionsSqLite(path, new FileDb().Name);

                options.UseSqlite(optionsSqLite.GetConnectionString());
            });
        }

        protected override void InitializeContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new TradesCoordinatorApiModule());
            builder.RegisterModule(new InfrastructureCommonModule());
            builder.RegisterModule(new MicroservicesLoggingModule());
        }

        protected override void ConfigureDb(IApplicationBuilder app)
        {
            Batteries.Init();

            app.UpgradeDb();

            var synchronizationRepository = app.ApplicationServices.GetRequiredService<ISynchronizationRepository>();
            synchronizationRepository.Initialization();
        }

        protected override void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<TradesInsertedIntegrationEvent, TradesInsertedIntegrationEventHandler>();
        }

        protected override void ResolveContainer(IContainer container)
        {
            try
            {
                // var s = container.Resolve<ICurrencyPairService>();
            }
            catch (Exception e)
            {
                var s = e;
            }
        }
    }
}

