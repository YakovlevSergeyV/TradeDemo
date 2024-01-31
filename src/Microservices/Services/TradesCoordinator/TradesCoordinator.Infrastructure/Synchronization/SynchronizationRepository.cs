namespace TradesCoordinator.Infrastructure.Synchronization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using global::Infrastructure.Common.Ambient;
    using Microsoft.Extensions.Logging;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.EntityContext;
    using TradesCoordinator.Infrastructure.Extensions;
    using TradesCoordinator.Infrastructure.Services;

    public class SynchronizationRepository : ISynchronizationRepository
    {
        private readonly ConcurrentDictionary<string, Tuple<ExchangeInfo, ISynchronizationManagerExchange>> _exchanges;
        private readonly ConcurrentDictionary<string, Tuple<Service, DateTime>> _services;

        private readonly CoordinatorContext _context;
        private readonly ISymbolDictionary _symbolDictionary;
        private readonly ILoggerFactory _loggerFactory;

        public SynchronizationRepository(
            CoordinatorContext context,
            ISymbolDictionary symbolDictionary,
            ILoggerFactory loggerFactory)
        {
            _exchanges = new ConcurrentDictionary<string, Tuple<ExchangeInfo, ISynchronizationManagerExchange>>();
            _services = new ConcurrentDictionary<string, Tuple<Service, DateTime>>();

            _context = context;
            _symbolDictionary = symbolDictionary;
            _loggerFactory = loggerFactory;
        }

        public void Initialization()
        {
            foreach (var exchange in _context.Exchanges.ToList())
            {
                AddExchange(exchange);

                var currencyPairs = _context.CurrencyPairs
                    .Where(x => x.ExchangeName == exchange.ExchangeName);

                foreach (var currencyPair in currencyPairs)
                {
                    AddSymbol(exchange, currencyPair, TimeProvider.Current.UtcNow);
                }
            }
        }

        public IEnumerable<ExchangeInfo> Exchanges
        {
            get { return _exchanges.Values.Select(x => x.Item1).ToList(); }
        }

        public ISynchronizationManagerExchange GetExchangeSynchronization(string exchange)
        {
            return _exchanges.ContainsKey(exchange) ? _exchanges[exchange].Item2 : null;
        }

        public void AddExchange(ExchangeInfo exchange)
        {
            if (_exchanges.ContainsKey(exchange.ExchangeName)) return;
            var tuple = new Tuple<ExchangeInfo, ISynchronizationManagerExchange>(exchange,
                new SynchronizationManagerExchange(exchange, _symbolDictionary, _loggerFactory));
            _exchanges.GetOrAdd(exchange.ExchangeName, tuple);
        }

        public void UpdateExchange(ExchangeInfo exchange)
        {
            if (!_exchanges.ContainsKey(exchange.ExchangeName)) return;
            _exchanges[exchange.ExchangeName].Item2.UpdateExchange(exchange);
            _exchanges[exchange.ExchangeName].Item1.Update(exchange);
        }

        public void DeleteExchange(ExchangeInfo exchange)
        {
            if (!_exchanges.ContainsKey(exchange.ExchangeName)) return;
            if (!_exchanges.TryRemove(exchange.ExchangeName, out _))
            {
                //Надо записать в лог
            }
        }

        public void AddSymbol(ExchangeInfo exchange, SymbolInfo symbolInfo, DateTime date)
        {
            if (_exchanges.ContainsKey(exchange.ExchangeName))
            {
                _exchanges[exchange.ExchangeName].Item2.AddCurrencyPair(symbolInfo, date);
            }
        }

        public void DeleteSymbol(ExchangeInfo exchange, SymbolInfo symbolInfo)
        {
            if (_exchanges.ContainsKey(exchange.ExchangeName))
            {
                _exchanges[exchange.ExchangeName].Item2.DeleteCurrencyPair(symbolInfo);
            }
        }

        public void AddService(Service service, DateTime date)
        {
            if (!_services.ContainsKey(GetServiceKey(service)))
            {
                _services.GetOrAdd(GetServiceKey(service), new Tuple<Service, DateTime>(service, date));
            }
            else
            {
                _services[GetServiceKey(service)] = new Tuple<Service, DateTime>(service, date);
            }
        }

        public bool ExistsService(Service service)
        {
            return _services.ContainsKey(GetServiceKey(service));
        }

        public IEnumerable<Service> Services
        {
            get { return _services.Values.Select(x => x.Item1); }
        }

        private static string GetServiceKey(Service service)
        {
            return $"{service.HostName.ToUpper()}-{service.ExchangeName.ToUpper()}";
        }
    }
}
