namespace Infrastructure.Common.Providers
{
    using Autofac;

    public class ProviderContainer : IProviderContainer
    {
        public IContainer Container { get; set; }
    }
}
