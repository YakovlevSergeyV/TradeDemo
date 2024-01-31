
namespace Infrastructure.Common.Providers
{
    using Autofac;

    public interface IProviderContainer
    {
        IContainer Container { get; set; }
    }
}
