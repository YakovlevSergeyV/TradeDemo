namespace TradesCoordinator.Infrastructure.UnitTest.Infrastructure.Synchronization
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Shouldly;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.Services;
    using TradesCoordinator.Infrastructure.Synchronization;
    using TradesCoordinator.Infrastructure.UnitTest.Infrastructure.Synchronization.Templates;

    [TestClass]
    public class SynchronizationManagerExchangeTest 
    {
        private Mock<ISymbolDictionary> _symbolExclude;
        private Mock<ILoggerFactory> _loggerFactory;
        private readonly ExchangeInfo _exchange;

        public SynchronizationManagerExchangeTest()
        {
            _symbolExclude = new Mock<ISymbolDictionary>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _exchange = new ExchangeInfo();
        }

        [TestMethod]
        public void NextCurrencyPair_TooManyRequests()
        {
            // Given
            var instance = CreateInstance();
            var currencyPair = "currencyPair";
            var service = new Service{ Guid = "Guid", ExchangeName = "ExchangeName", HostName = "HostName" };
            instance.NotSuccessfulPerformance(service, currencyPair, TemplateManager.GetExceptionTooManyRequests());

            // When
            var currencyPairActual = instance.NextCurrencyPair(service);
            
            // Then
            currencyPairActual.CurrencyPairName.ShouldBe(null);
        }


        private SynchronizationManagerExchange CreateInstance()
        {
            return new SynchronizationManagerExchange(_exchange, _symbolExclude.Object, _loggerFactory.Object);
        }
    }
}
