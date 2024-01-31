namespace TradesReader.Api.Infrastructure.Command
{
    using System.Threading.Tasks;
    using TradesReader.Api.Dto;

    public interface IReadData
    {
        Task<ResultPerformance> Run();
    }
}
