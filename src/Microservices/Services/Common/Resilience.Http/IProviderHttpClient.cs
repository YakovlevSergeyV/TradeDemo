
namespace Resilience.Http
{
    using System.Net.Http;

    public interface IProviderHttpClient
    {
        HttpClient GetHttpClient();
    }
}
