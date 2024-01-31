namespace Microservices.Common
{
    using System;
    using System.IO.Compression;
    using System.Threading;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using EventBus;
    using EventBus.Abstractions;
    using EventBusRabbitMQ;
    using global::Infrastructure.Common.Providers;
    using Microservices.Common.Info;
    using Microservices.Common.Infrastructure.Enrichers;
    using Microservices.Common.Infrastructure.Filters;
    using Microservices.Common.Infrastructure.Middlewares;
    using Microservices.Common.Infrastructure.Model;
    using Microservices.Logging.Abstract;
    using Microservices.Logging.Manager;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using RabbitMQ.Client;
    using Resilience.Http;
    using Serilog;
    using Serilog.Events;

    public abstract class CommonStartup<T>
        where T : CommonSettingsApplication
    {
        private IWebHostEnvironment _environment;

        protected CommonStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
            InitializeSwaggerInfo();
        }

        public IConfiguration Configuration { get; }

        protected abstract string ApplicationName { get; }

        protected virtual void InitializeSwaggerInfo()
        {
            var exchangeName = Configuration["ExchangeName"];
            var currencyPairName = Configuration["CurrencyPairName"];
            var subscriptionClientName = Configuration["SubscriptionClientName"];

            if (!string.IsNullOrEmpty(currencyPairName))
            {
                currencyPairName = currencyPairName.ToUpper();
            }

            if (!string.IsNullOrEmpty(exchangeName) && !string.IsNullOrEmpty(subscriptionClientName))
            {
                exchangeName = exchangeName.Substring(0, 1).ToUpper() +
                               exchangeName.Substring(1).ToLower();

                Configuration["SubscriptionClientName"] =
                    subscriptionClientName.Replace("Service", "") + exchangeName + currencyPairName + "Service";
            }

            var exchangeNameWithSpace = string.Empty;
            if (!string.IsNullOrEmpty(exchangeName))
            {
                exchangeNameWithSpace = $" {exchangeName}";
            }

            var currencyPairNameWithSpace = string.Empty;
            if (!string.IsNullOrEmpty(currencyPairName))
            {
                currencyPairNameWithSpace = $" {currencyPairName}";
            }


            SwaggerInfo.Doc.Title = $"{ApplicationName}{exchangeNameWithSpace}{currencyPairNameWithSpace} HTTP API";
            SwaggerInfo.Doc.Description =
                $"The {ApplicationName}{exchangeNameWithSpace}{currencyPairNameWithSpace} Service HTTP API";
            SwaggerInfo.Doc.Version = "v1";

            SwaggerInfo.SecurityScopes.Add($"{ApplicationName}{exchangeName}{currencyPairName}", "Stochastic API");

            SwaggerInfo.Endpoint.Name = $"{ApplicationName}{exchangeName}{currencyPairName}.Api V1";
            SwaggerInfo.Endpoint.AuthClientId = $"{ApplicationName}{exchangeName}{currencyPairName}SwaggerUi";
            SwaggerInfo.Endpoint.AuthAppName =
                $"{ApplicationName}{exchangeNameWithSpace}{currencyPairNameWithSpace} Swagger UI";

            SwaggerInfo.FilterAppKeyName = $"{{ApplicationName}}{exchangeName}{currencyPairName}Api";
        }

        #region ConfigureServices

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var serviceGuid = Guid.NewGuid().ToString("N");

            services.AddControllers().AddNewtonsoftJson();

            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.TryAddSingleton<HttpSettings>();
            services.AddSingleton<IProviderHttpClient, ProviderHttpClient>(sp =>
            {
                var httpSettings = sp.GetRequiredService<HttpSettings>();
                return new ProviderHttpClient(httpSettings);
            });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton(new ServiceIdentity(serviceGuid));
            services.TryAddSingleton(new GarbageCollectorOptions());

            services.Configure<T>(Configuration);

            RegisterLogger(services, serviceGuid);

            //services.AddMvc(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
            //    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            //    .AddControllersAsServices();

            if (Configuration.GetValue<string>("UseCompression") == bool.TrueString)
            {
                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Optimal;
                });

                services.AddResponseCompression(options =>
                {
                    var mimeTypes = new[]
                    {
                        "text/plain",
                        "application/json",
                        "text/json"
                    };
                    options.MimeTypes = mimeTypes;
                    options.EnableForHttps = true;
                    options.Providers.Add<GzipCompressionProvider>();
                });
            }

            RegisterAuthService(services);

            RegisterEventBus(services);

            RegisterSwagger(services);

            RegisterServicesAdditional(services);

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        x => x.AllowAnyOrigin()
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //            .AllowCredentials());
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    x => x.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            if (Configuration.GetValue<string>("UseResilientHttp") == bool.TrueString)
            {
                //services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>(sp =>
                //{
                //    var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                //    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                //    var provider = sp.GetRequiredService<IProviderHttpClient>();

                //    var retryCount = 6;
                //    if (!string.IsNullOrEmpty(Configuration["HttpClientRetryCount"]))
                //    {
                //        retryCount = int.Parse(Configuration["HttpClientRetryCount"]);
                //    }

                //    var exceptionsAllowedBeforeBreaking = 5;
                //    if (!string.IsNullOrEmpty(Configuration["HttpClientExceptionsAllowedBeforeBreaking"]))
                //    {
                //        exceptionsAllowedBeforeBreaking =
                //            int.Parse(Configuration["HttpClientExceptionsAllowedBeforeBreaking"]);
                //    }

                //    return new ResilientHttpClientFactory(provider, logger, httpContextAccessor,
                //        exceptionsAllowedBeforeBreaking, retryCount);
                //});
                //services.AddSingleton<IHttpClient, ResilientHttpClient>(sp =>
                //    sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());
            }
            else
            {
                services.AddSingleton<IHttpClient, StandardHttpClient>();
            }

            //configure autofac
            var builder = new ContainerBuilder();

            InitializeContainer(builder);

            builder.Populate(services);

            builder.RegisterType<ProviderContainer>().As<IProviderContainer>().SingleInstance();
            var container = builder.Build();

            var providerContainer = container.Resolve<IProviderContainer>();
            providerContainer.Container = container;

            LogManager.LoggerManager = container.Resolve<ILoggerManager>();
            if (!string.IsNullOrEmpty(Configuration["LogLevelDefault"])
                && Enum.TryParse(Configuration["LogLevelDefault"], out LogLevel level))
            {
                LogManager.LoggerManager.LoggerLevel = level;
            }

            ResolveContainer(container);

            return new AutofacServiceProvider(container);
        }
        
        protected virtual void RegisterLogger(IServiceCollection services, string serviceGuid)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.With(new CorrelationIdEnricher(services))
                //.Enrich.WithProperty("ApplicationVersion", config.ApplicationVersion)
                .Enrich.WithProperty("ApplicationId", serviceGuid)
                .Enrich.WithProperty("ApplicationName", _environment.ApplicationName)
                //.ReadFrom.Configuration(hostingContext.Configuration)
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(x =>
                    {
                        if (LogManager.LoggerManager.LoggerLevel == LogLevel.None) return false;
                        if ((int) LogManager.LoggerManager.LoggerLevel > (int) x.Level) return false;
                        return true;
                    })
                    .WriteTo.Seq($"http://{Configuration["SeqConnection"]}")
                )
                // .MinimumLevel.Error().WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "logs/event_{Date}.txt"), outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] [{EventId}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
                // loggingBuilder.AddConsole();
                // loggingBuilder.AddDebug();
            });
        }

        protected virtual void RegisterAuthService(IServiceCollection services)
        {
        }

        protected virtual void RegisterEventBus(IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });


            var subscriptionClientName = Configuration["SubscriptionClientName"];


            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 2;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope,
                    eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }

        protected virtual void RegisterSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(SwaggerInfo.Doc.Version,
                    new OpenApiInfo
                    {
                        Title = SwaggerInfo.Doc.Title,
                        Version = SwaggerInfo.Doc.Version,
                        Description = SwaggerInfo.Doc.Description,
                    });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        protected virtual void RegisterServicesAdditional(IServiceCollection services)
        {
        }

        protected abstract void InitializeContainer(ContainerBuilder container);

        protected virtual void ResolveContainer(IContainer container)
        {
            try
            {
                //var s = container.Resolve<IHttpContextAccessor>();
            }
            catch (Exception e)
            {
                var s = e;
            }
        }

        #endregion


        #region Configure

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (Configuration.GetValue<string>("UseCompression") == bool.TrueString)
            {
                app.UseResponseCompression();
                app.UseMiddleware<GzipRequestMiddleware>();
            }

            var httpSettings = app.ApplicationServices.GetService<HttpSettings>();
            if (!string.IsNullOrEmpty(Configuration.GetValue<string>("Proxy")))
            {
                httpSettings.Proxy = Configuration.GetValue<string>("Proxy");
            }

            if (Configuration.GetValue<int>("RequestTimeoutSecond") > 0)
            {
                httpSettings.Timeout = new TimeSpan(0, 0, 0, Configuration.GetValue<int>("RequestTimeoutSecond"));
            }

            // app.UseHsts();
            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            // app.UseCorrelationId();

            app.UseMiddleware<GarbageCollectorMiddleware>();
            app.UseMiddleware<SerilogMiddleware>();

            ConfigureSettings(app);

            ConfigureLogger(app);

            ConfigureAuth(app);

            app.UseMvcWithDefaultRoute();

            app.UseCors("CorsPolicy");

            ConfigureSwagger(app);

            ConfigureDb(app);

            ConfigureServiceRun(app);

            ConfigureEventBus(app);
        }

        protected virtual void ConfigureSettings(IApplicationBuilder app)
        {
        }

        protected virtual void ConfigureLogger(IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(GetType().Name);

            var sleepInMill = Configuration["SleepStartInMilliseconds"];
            if (!string.IsNullOrEmpty(sleepInMill))
            {
                logger.LogWarning($"Задержка запуска сервиса {ApplicationName} на {sleepInMill} мл. сек.");
                Thread.Sleep(TimeSpan.FromMilliseconds(int.Parse(sleepInMill)));
            }

            logger.LogWarning($"Запуск сервиса {ApplicationName}");
        }

        protected virtual void ConfigureSwagger(IApplicationBuilder app)
        {
            var pathBase = Configuration["PATH_BASE"];
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                        SwaggerInfo.Endpoint.Name);
                    c.OAuthClientId(SwaggerInfo.Endpoint.AuthClientId);
                    c.OAuthAppName(SwaggerInfo.Endpoint.AuthAppName);
                });
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }
        }

        protected virtual void ConfigureDb(IApplicationBuilder app)
        {
        }

        protected virtual void ConfigureServiceRun(IApplicationBuilder app)
        {
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
        }

        #endregion
    }
}
