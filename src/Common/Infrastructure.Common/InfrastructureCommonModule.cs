
namespace Infrastructure.Common
{
    using Autofac;
    using Infrastructure.Common.Utils;

    public class InfrastructureCommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystemService>().As<IFileSystemService>().SingleInstance();
            builder.RegisterType<ComputerProperties>().As<IComputerProperties>().SingleInstance();
            builder.RegisterType<Zip>().As<IZip>().SingleInstance();
        }
    }
}
