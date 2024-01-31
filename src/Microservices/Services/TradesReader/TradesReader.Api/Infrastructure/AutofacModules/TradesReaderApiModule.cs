namespace TradesReader.Api.Infrastructure.AutofacModules
{
    using Autofac;
    using global::Infrastructure.Common.Providers;
    using ServiceWorker.Abstract;
    using TradesReader.Api.Dto;
    using TradesReader.Api.Infrastructure.Command;
    using TradesReader.Api.Infrastructure.Services;
    using TradesReader.Api.Request;

    public class TradesReaderApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();
            builder.RegisterType<SymbolDictionary>().As<ISymbolDictionary>().SingleInstance();
            builder.RegisterType<Request>().As<IRequest>();
            builder.RegisterType<ServiceInfo>().SingleInstance();
            builder.RegisterType<CommandServiceTrade>().As<ICommandService>().SingleInstance();
            builder.RegisterType<ReadTrade>().As<IReadData>();
        }
    }
}
