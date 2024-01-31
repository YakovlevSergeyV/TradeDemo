
namespace Logging.Desktop
{
    using Autofac;
    using Logging.Desktop.Abstract;
    using Logging.Desktop.Infrastructure;

    public class DesktopLoggingModulee : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LogAdapter>().As<ILogAdapter>().SingleInstance();
        }
    }
}
