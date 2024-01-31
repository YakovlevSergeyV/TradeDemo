
namespace Infrastructure.Common.Providers
{
    using Microsoft.Extensions.Configuration;

    public class ProviderConfiguration : IProviderConfiguration
    {
        public IConfiguration Configuration { get; set; }
    }
}
