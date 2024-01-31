namespace TradesReader.Api.Infrastructure.Command
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Infrastructure.Common.Ambient;
    using global::Infrastructure.Common.Convert;
    using GlobalSpace.Common.Guardly;
    using Microservices.Common.Infrastructure.Model;
    using Microsoft.Extensions.Logging;
    using ServiceWorker.Abstract;
    using TradesReader.Api.Dto;
    using TradesReader.Api.Request;

    internal class CommandServiceTrade : ICommandService
    {
        private long _timeStampNext;
        private bool _execute;
        private bool _initialized;
        private readonly ServiceIdentity _serviceIdentity;
        private readonly ServiceInfo _serviceInfo;
        private readonly IRequest _request;
        private readonly ILogger<CommandServiceTrade> _logger;
        private readonly Func<string, IReadData> _readDataFactory;

        public CommandServiceTrade(
            ServiceIdentity serviceIdentity,
            ServiceInfo serviceInfo,
            IRequest request,
            Func<string, IReadData> readDataFactory,
            ILogger<CommandServiceTrade> logger)
        {
            Guard.Argument(() => serviceIdentity, Is.NotNull);
            Guard.Argument(() => serviceInfo, Is.NotNull);
            Guard.Argument(() => request, Is.NotNull);
            Guard.Argument(() => readDataFactory, Is.NotNull);
            Guard.Argument(() => logger, Is.NotNull);

            _serviceIdentity = serviceIdentity;
            _serviceInfo = serviceInfo;
            _request = request;
            _readDataFactory = readDataFactory;
            _logger = logger;

            _initialized = false;
            _execute = false;
        }

        public void Run()
        {
            if (_execute) return;
            if (ConvertDateTime.DateToUnixTimeStampMilliseconds(TimeProvider.Current.UtcNow) < _timeStampNext) return;
            _execute = true;
            try
            {
                var task = RunInner();
                task.Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Летна. Ошибка чтения данных.");
            }
            finally
            {
                _execute = false;
            }
        }

        private async Task RunInner()
        {
            var resultInit = await Initializing();
            if (!resultInit) return;
            var currentDate = TimeProvider.Current.UtcNow;

            _logger.LogTrace($"Летна.Читаем с координатора валютную пару {TimeProvider.Current.UtcNow:HH:mm:ss.fff tt}");

            var symbol = await _request.GetTradeNextSymbolsAsync(_serviceInfo);
            if (symbol == null) return;
            _timeStampNext = symbol.TimeStampNext;
            if (string.IsNullOrEmpty(symbol.CurrencyPairName)) return;

            _logger.LogDebug(
                $"Летна. Прочитали с координатора валютную пару {TimeProvider.Current.UtcNow:HH:mm:ss.fff tt}, время следующего запуска {ConvertDateTime.UnixTimeStampMillisecondsToDateTime(_timeStampNext):HH:mm:ss.fff tt}");

            var readData = _readDataFactory.Invoke(symbol.CurrencyPairName);

            var result = await readData.Run();

            if (result.Successful)
            {
                var performance = new PerformanceSuccessful
                {
                    CurrencyPairName = symbol.CurrencyPairName ,
                    ExchangeName= _serviceInfo.ExchangeName,
                    HostName= _serviceInfo.HostName,
                    Timestamp = ConvertDateTime.DateToUnixTimeStampMilliseconds(currentDate)
                };
                await _request.TradePerformanceSuccessfulAsync(performance);
            }
            else
            {
                var performance = new PerformanceNotSuccessful
                {
                    CurrencyPairName = symbol.CurrencyPairName,
                    ExchangeName = _serviceInfo.ExchangeName,
                    HostName = _serviceInfo.HostName,
                    Exception = result.Exception.ToString()
                };
                await _request.TradePerformanceNotSuccessfulAsync(performance);
            }
        }

        private async Task<bool> Initializing()
        {
            if (_initialized) return _initialized;
            try
            {
                //Проверка регистрации сервиса
                _serviceInfo.Guid = _serviceIdentity.Guid;
                var result = await _request.GetTradeServiceStartedAsync(_serviceInfo);
                if (!result)
                {
                    //Регистрируем сервис
                    _logger.LogWarning("Летна. Регистрируем сервис {0}-{1}.", _serviceInfo.HostName, _serviceInfo.ExchangeName);
                    await _request.TradeRegisterServiceAsync(_serviceInfo);
                    _initialized = true;
                }
                else
                {
                    var sleep = 1;
                    _logger.LogWarning(
                        "Летна. Сервис {0}-{1} уже зарегистрирован. Повторная попытка произойдет через {2} секунду.",
                        _serviceInfo.HostName, _serviceInfo.ExchangeName, sleep);
                    Thread.Sleep(sleep * 1000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Летна. Ошибка регистрации сервиса.");
                Thread.Sleep(5000);
                return false;
            }

            return _initialized;
        }
    }
}
