
namespace Infrastructure.Common.Providers
{
    using Microsoft.Extensions.Configuration;

    public interface IProviderConfiguration
    {
        IConfiguration Configuration { get; set; }
    }
}
