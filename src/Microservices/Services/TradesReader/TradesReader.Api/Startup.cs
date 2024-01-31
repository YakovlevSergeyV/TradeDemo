namespace TradesReader.Api
{
    using System;
    using System.Reflection;
    using System.Threading;
    using Autofac;
    using global::Infrastructure.Common;
    using global::Infrastructure.Common.Utils;
    using Microservices.Common;
    using Microservices.Logging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ServiceWorker;
    using ServiceWorker.Abstract;
    using TradesReader.Api.Dto;
    using TradesReader.Api.Infrastructure.AutofacModules;

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

        protected override void InitializeContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new TradesReaderApiModule());
            builder.RegisterModule(new InfrastructureCommonModule());
            builder.RegisterModule(new MicroservicesLoggingModule());
            builder.RegisterModule(new ServiceWorkerModule());
        }

        protected override void RegisterServicesAdditional(IServiceCollection services)
        {
            services.Configure<UrlsConfig>(Configuration.GetSection("Urls"));
        }

        protected override void ConfigureSettings(IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetRequiredService<IOptions<SettingsApplication>>().Value;
            settings.ExchangeName = settings.ExchangeName.ToUpper();

            var computerProperties = app.ApplicationServices.GetService<IComputerProperties>();

            var serviceInfo = app.ApplicationServices.GetService<ServiceInfo>();
            serviceInfo.ExchangeName = settings.ExchangeName;
            serviceInfo.HostName = computerProperties.GetInfo().HostName;
        }

        protected override void ConfigureServiceRun(IApplicationBuilder app)
        {
            var worker = app.ApplicationServices.GetService<IWorker>();
            worker.HeartBeatCycleInMs = 100;
            var service = app.ApplicationServices.GetService<IService>();
            service.Start();
        }

        protected override void ConfigureLogger(IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetRequiredService<IOptions<SettingsApplication>>().Value;
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(GetType().Name);

            var sleepInMill = Configuration["SleepStartInMilliseconds"];
            if (!string.IsNullOrEmpty(sleepInMill))
            {
                logger.LogWarning($"Задержка запуска сервиса {ApplicationName} {settings.ExchangeName} на {sleepInMill} мл. сек.");
                Thread.Sleep(TimeSpan.FromMilliseconds(int.Parse(sleepInMill)));
            }

            logger.LogWarning($"Запуск сервиса {ApplicationName} {settings.ExchangeName}");
        }

        protected override void ResolveContainer(IContainer container)
        {
            //try
            //{
            //    var factory = new ConnectionFactory
            //    {
            //        HostName = "213.166.70.92", //"rabbit",
            //        Port = AmqpTcpEndpoint.UseDefaultPort,
            //        UserName = "user",
            //        Password = "user",
            //         VirtualHost = "/",
            //        //    AutomaticRecoveryEnabled = true,
            //        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            //    };

            //    var connection = factory.CreateConnection();

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
                // var s = container.Resolve<ServiceIdentity>();
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

