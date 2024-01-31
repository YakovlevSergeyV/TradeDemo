namespace TradesCoordinator.Infrastructure.Synchronization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TradesCoordinator.Infrastructure.Dto;

    public interface ISynchronizationManager
    {
        Task RegistrationService(Service service);

        Task<bool> StartedService(Service service);
        IEnumerable<Service> Services { get; }

        Task<SymbolNext> NextCurrencyPair(Service service);

        Task PerformanceSuccessful(PerformanceSuccessful request);

        Task PerformanceNotSuccessful(PerformanceNotSuccessful request);

        void SetDataStorage(string exchange, string currencyPair);

        void SwitchIn(Symbol symbol);

        void SwitchOff(Symbol symbol);
        bool GetRunning(Symbol symbol);
    }
}
