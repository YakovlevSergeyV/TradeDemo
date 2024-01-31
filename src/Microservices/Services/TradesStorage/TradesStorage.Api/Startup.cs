namespace TradesStorage.Api
{
    using System;
    using System.Reflection;
    using Autofac;
    using global::Infrastructure.Common;
    using Microservices.Common;
    using Microservices.Logging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using SQLitePCL;
    using TradesStorage.Api.Infrastructure.AutofacModules;
    using TradesStorage.Api.Infrastructure.UpgradeDb;

    public class Startup : CommonStartup<SettingsApplication>
    {
        private static readonly string applicationName;

        static Startup()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var name = currentAssembly.GetName().Name;
            applicationName = name.Replace(".Api", "");
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment)
        {
        }

        protected override string ApplicationName => applicationName;

        protected override void InitializeContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new TradesStorageApiModule());
            builder.RegisterModule(new InfrastructureCommonModule());
            builder.RegisterModule(new MicroservicesLoggingModule());
        }

        protected override void ConfigureDb(IApplicationBuilder app)
        {
            Batteries.Init();

            app.UpgradeDb();
        }

        protected override void ConfigureEventBus(IApplicationBuilder app)
        {
            //var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            //eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
        }

        protected override void ResolveContainer(IContainer container)
        {
            //try
            //{
            //    var factory = new ConnectionFactory
            //    {
            //        HostName = "localhost",
            //        Port = 5672,
            //        UserName = "user",
            //        Password = "user",
            //    //    VirtualHost = "/",
            //    //    AutomaticRecoveryEnabled = true,
            //        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            //    };
            //  // var connection = factory.CreateConnection();

            //}
            //catch (Exception e)
            //{
            //    var x = e;
            //    while (x.InnerException != null)
            //    {
            //        x = x.InnerException;
            //    }
            //    throw new ArgumentException(x.Message);
            //}
            try
            {
               // var s = container.Resolve<IEventBus>();
            }
            catch (Exception e)
            {
                var x = e;
                while (x.InnerException != null)
                {
                    x = x.InnerException;
                }
                throw new ArgumentException(x.Message); 
            }
        }
    }
}

