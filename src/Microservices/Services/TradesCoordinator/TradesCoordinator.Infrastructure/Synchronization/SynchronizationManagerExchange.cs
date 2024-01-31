namespace TradesCoordinator.Infrastructure.Synchronization
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using global::Infrastructure.Common.Ambient;
    using global::Infrastructure.Common.Convert;
    using global::Infrastructure.Common.Extensions;
    using Microsoft.Extensions.Logging;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.Extensions;
    using TradesCoordinator.Infrastructure.Services;

    public class SynchronizationManagerExchange : ISynchronizationManagerExchange
    {
        private readonly ConcurrentDictionary<string, ServiceTime> _lastDateServices;
        private readonly ConcurrentDictionary<string, ServiceState> _services;
        private readonly ConcurrentDictionary<string, SymbolState> _symbols;

        private readonly ExchangeInfo _exchange;
        private readonly ISymbolDictionary _symbolExclude;
        private readonly ILogger _logger;

        public SynchronizationManagerExchange(
            ExchangeInfo exchange,
            ISymbolDictionary symbolExclude,
            ILoggerFactory loggerFactory)
        {
            _exchange = exchange;
            _symbolExclude = symbolExclude;
            _logger = loggerFactory.CreateLogger(typeof(SynchronizationManagerExchange));

            _symbols = new ConcurrentDictionary<string, SymbolState>();
            _services = new ConcurrentDictionary<string, ServiceState>();
            _lastDateServices = new ConcurrentDictionary<string, ServiceTime>();
        }

        public void AddCurrencyPair(SymbolInfo symbolInfo, DateTime date)
        {
            var key = GetSymbolKey(symbolInfo);
            if (!_symbols.ContainsKey(key))
            {
                var symbolState = new SymbolState
                {
                    CurrencyPairName = symbolInfo.CurrencyPairName,
                    ExchangeName = symbolInfo.ExchangeName,
                    Data = date
                };
                _logger.LogDebug($"TradesCoordinator-SynchronizationManagerExchange-AddCurrencyPair:date={date.UpToSecondsToString()}");
                _symbols.GetOrAdd(key, symbolState);
            }
        }

        public void DeleteCurrencyPair(SymbolInfo symbolInfo)
        {
            var key = GetSymbolKey(symbolInfo);

            if (_symbols.ContainsKey(key))
            {
                if (!_symbols.TryRemove(key, out _))
                {
                    //Надо записать в лог
                }
            }
        }

        public void UpdateExchange(ExchangeInfo exchangeParam)
        {
            _logger.LogDebug($"TradesCoordinator-SynchronizationManagerExchange-UpdateExchange");
            _exchange.Update(exchangeParam);
        }

        public SymbolNext NextCurrencyPair(Service service)
        {
            var curentDate = TimeProvider.Current.UtcNow;

            _logger.LogDebug($"TradesCoordinator-SynchronizationManagerExchange-NextCurrencyPair:curentDate={curentDate.UpToSecondsToString()}");

            var symbol = NextCurrencyPair(service, curentDate);

            _logger.LogDebug($"TradesCoordinator-SynchronizationManagerExchange-NextCurrencyPair:TimeStampNext={ConvertDateTime.UnixTimeStampMillisecondsToDateTime(symbol.TimeStampNext).UpToSecondsToString()}");

            _lastDateServices[service.HostName].TimestampLastRun =
                ConvertDateTime.DateToUnixTimeStampMilliseconds(curentDate);
            _lastDateServices[service.HostName].TimestampNextRun = symbol.TimeStampNext;
            return symbol;
        }

        private static bool ExceptionValidate(string exceptionMessage, Service service)
        {
            return service.ExchangeName == "BITFINEX"
                   && !string.IsNullOrEmpty(exceptionMessage)
                   && exceptionMessage.IndexOf("Too Many Requests", StringComparison.Ordinal) >= 0;
        }

        private SymbolNext NextCurrencyPair(Service service, DateTime curentDate)
        {
            long timeStampLast = 0;
            var exception = string.Empty;

            var timeStampNext = ConvertDateTime.DateToUnixTimeStampMilliseconds(curentDate.AddMilliseconds(_exchange.HeartBeatCycleInMs));

            if (!_lastDateServices.ContainsKey(service.HostName))
            {
                _lastDateServices.GetOrAdd(service.HostName, new ServiceTime());
            }
            timeStampLast = _lastDateServices[service.HostName].TimestampLastRun;

            if (_services.ContainsKey(service.HostName))
            {
                exception = _services[service.HostName].Exception;
            }

            if (timeStampLast != 0)
            {
                var timeStampCurrent = ConvertDateTime.DateToUnixTimeStampMilliseconds(TimeProvider.Current.UtcNow);
                if (timeStampCurrent - timeStampLast < _exchange.HeartBeatCycleInMs)
                {
                    return new SymbolNext
                    {
                        TimeStampNext = GetTimeStampNext(timeStampNext)
                    };
                }
            }

            if (ExceptionValidate(exception, service))
            {
                //Не даем список валютных пар до следующей минуты
                var сurrent = TimeProvider.Current.UtcNow;
                var dt = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(timeStampLast).AddMinutes(1);
                var dateNew = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
                if (сurrent < dateNew)
                {
                    return new SymbolNext
                    {
                        TimeStampNext = ConvertDateTime.DateToUnixTimeStampMilliseconds(dateNew)
                    };
                }
            }

            //logger.LogWarning(
            //    $"Летна. Запуск усечения базы данных {symbol.ExchangeName}-{symbol.CurrencyPairName} {length}byte");

            var value =
                _symbols.Values
                    .Where(
                        x => !_symbolExclude.Symbols.ContainsKey(GetSymbolKey(x))
                             &&
                             (x.DataStop != DateTime.MinValue
                              | x.DataStart.AddSeconds(60) <= TimeProvider.Current.UtcNow)
                    )
                    .OrderBy(x => x.DataStop)
                    .ThenBy(x => x.Data)
                    .ThenBy(x => x.CurrencyPairName)
                    .FirstOrDefault();

            if (value == null)
            {
                return new SymbolNext
                {
                    TimeStampNext = GetTimeStampNext(timeStampNext)
                };
            }

            value.DataStop = DateTime.MinValue;
            value.DataStart = TimeProvider.Current.UtcNow;

            return new SymbolNext
            {
                CurrencyPairName = value.CurrencyPairName,
                TimeStampNext = GetTimeStampNext(timeStampNext)
            };
        }

        private long GetTimeStampNext(long timeStampNext)
        {
            var items = _lastDateServices.Where(x => !x.Value.isException).ToList();
            if (items.Count <= 1) return timeStampNext;
            var inteval = _exchange.HeartBeatCycleInMs / _lastDateServices.Count;
            var timeStamp = items.Max(x => x.Value.TimestampNextRun);

            if (timeStamp + inteval < timeStampNext)
            {
                timeStamp = timeStamp + _exchange.HeartBeatCycleInMs + inteval;
            }
            else
            {
                timeStamp = timeStamp + inteval;
            }
            return timeStamp < timeStampNext ? timeStampNext : timeStamp;
        }

        public void SuccessfulPerformance(Service service, string currencyPair, long timeStamp)
        {
            var key = GetSymbolKey(service.ExchangeName, currencyPair);

            if (_symbols.ContainsKey(key))
            {
                var value = _symbols[key];
                value.Data = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(timeStamp);
            }

            AddService(service);

            var serviceState = _services[service.HostName];
            serviceState.Exception = string.Empty;
            serviceState.LastPerformanceService = timeStamp;

            if (_lastDateServices.ContainsKey(service.HostName))
            {
                _lastDateServices[service.HostName].isException = false;
            }
        }

        public void NotSuccessfulPerformance(Service service, string currencyPair, string exception)
        {
            var now = TimeProvider.Current.UtcNow;

            AddService(service);

            var serviceState = _services[service.HostName];
            serviceState.LastPerformanceService = ConvertDateTime.DateToUnixTimeStampMilliseconds(now);
            serviceState.Exception = exception;

            var key = GetSymbolKey(service.ExchangeName, currencyPair);
            if (_symbols.ContainsKey(key))
            {
                _symbols[key].DataStop = now;
            }

            if (ExceptionValidate(exception, service) && _lastDateServices.ContainsKey(service.HostName))
            {
                _lastDateServices[service.HostName].isException = true;
            }
        }

        public DateTime LastDatePerformanceService(Service service)
        {
            if (_services.ContainsKey(service.HostName) && _services[service.HostName].LastPerformanceService != 0)
            {
                return ConvertDateTime.UnixTimeStampMillisecondsToDateTime(
                    _services[service.HostName].LastPerformanceService);
            }

            return DateTime.MinValue;
        }

        public void SavedStorage(string exchange, string currencyPair)
        {
            var key = GetSymbolKey(exchange, currencyPair);

            if (_symbols.ContainsKey(key))
            {
                _symbols[key].DataStop = TimeProvider.Current.UtcNow;
            }
        }

        private void AddService(Service service)
        {
            if (!_services.ContainsKey(service.HostName))
            {
                _services.GetOrAdd(service.HostName, new ServiceState
                {
                    HostName = service.HostName,
                    ExchangeName = service.ExchangeName
                });
            }
        }

        private static string GetSymbolKey(SymbolInfo symbol)
        {
            return GetSymbolKey(symbol.ExchangeName, symbol.CurrencyPairName);
        }

        private static string GetSymbolKey(string exchange, string currencyPair)
        {
            return $"{exchange}-{currencyPair}";
        }

        private class ServiceState : Service
        {
            public long LastPerformanceService { get; set; }
            public string Exception { get; set; }
        }
        
        private class ServiceTime
        {
            public long TimestampNextRun { get; set; }
            public long TimestampLastRun { get; set; }
            public bool isException { get; set; }
        }

        private class SymbolState : SymbolInfo
        {
            public DateTime DataStart { get; set; }
            public DateTime DataStop { get; set; }
            public DateTime Data { get; set; }
        }
    }
}
