namespace Microservices.Logging
{
    using Autofac;
    using Microservices.Logging.Abstract;
    using Microservices.Logging.Manager;

    public class MicroservicesLoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggerManager>().As<ILoggerManager>().SingleInstance();
        }
    }
}
