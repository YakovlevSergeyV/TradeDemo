namespace ServiceWorker
{
    using Autofac;
    using ServiceWorker.Abstract;
    using ServiceWorker.Infrastructure;

    public class ServiceWorkerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceDefault>().As<IService>().SingleInstance();
            builder.RegisterType<Worker>().As<IWorker>().SingleInstance();
            builder.RegisterType<ServiceModelFacade>().As<IServiceModelFacade>().SingleInstance();
        }
    }
}
