namespace TradesReader.Api.Infrastructure.Command
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Infrastructure.Common.Ambient;
    using global::Infrastructure.Common.Convert;
    using global::Infrastructure.Common.Extensions;
    using GlobalSpace.Common.Guardly;
    using Microsoft.Extensions.Logging;
    using TradesReader.Api.Dto;
    using TradesReader.Api.Infrastructure.Services;
    using TradesReader.Api.Request;

    internal class ReadTrade : IReadData
    {
        private readonly string _currencyPair;
        private readonly IRequest _request;
        private readonly ServiceInfo _serviceInfo;
        private readonly ISymbolDictionary _symbolDictionary;
        private readonly ILogger<ReadTrade> _logger;

        public ReadTrade(
            string currencyPair,
            IRequest request,
            ServiceInfo serviceInfo,
            ISymbolDictionary symbolDictionary,
            ILogger<ReadTrade> logger) 
        {
            Guard.Argument(() => currencyPair, Is.NotNullOrEmpty);
            Guard.Argument(() => request, Is.NotNull);
            Guard.Argument(() => serviceInfo, Is.NotNull);
            Guard.Argument(() => logger, Is.NotNull);

            _currencyPair = currencyPair;
            _request = request;
            _serviceInfo = serviceInfo;
            _symbolDictionary = symbolDictionary;
            _logger = logger;
        }

        public async Task<ResultPerformance> Run()
        {
            var symbol = new Symbol
            {
                ExchangeName = _serviceInfo.ExchangeName,
                CurrencyPairName = _currencyPair
            };
            try
            {
                _symbolDictionary.AddSymbol(symbol);
                return await RunInner();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Летна. Ошибка чтения данных {_serviceInfo.ExchangeName}-{_currencyPair}.");
                return new ResultPerformance
                {
                    Successful = false,
                    Exception = ex
                };
            }
            finally
            {
                _symbolDictionary.DeleteSymbol(symbol);
            }
        }

        private async Task<ResultPerformance> RunInner()
        {
            var parameters = await GetParameters();
            _logger.LogTrace($"Летна. Прочитали параметры: {_serviceInfo.ExchangeName}-{parameters}");

            var trades = await _request.GetMarketTradesAsync(_serviceInfo.ExchangeName, parameters);
            _logger.LogTrace(
                $"Летна. Прочитали с биржи {_serviceInfo.ExchangeName}-{_currencyPair} сделки в количестве {trades.Count()}");

            var timestamp = ConvertDateTime.DateToUnixTimeStampMilliseconds(parameters.TimeEnd);
            var dateMin = DateTime.MinValue;
            var dateMax = DateTime.MinValue;
            long id = 0;
            if (trades.Any())
            {
                await _request.TradeInsertAsync(_serviceInfo.ExchangeName, _currencyPair, timestamp, trades);
                _logger.LogTrace(
                    $"Летна. Зиписали в базу сделки с биржи {_serviceInfo.ExchangeName}-{_currencyPair} в количестве {trades.Count()}");

                dateMin = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(trades.Min(x => x.TimeStamp));
                dateMax = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(trades.Max(x => x.TimeStamp));
                id = trades.Max(x => x.Id);
            }
            else
            {
                await _request.TradeUpdateTimestampAsync(_serviceInfo.ExchangeName, _currencyPair, timestamp);
            }

            var interval = parameters.TimeEnd - parameters.TimeStart;
            var symbolInfo = parameters.SymbolInfo;
            symbolInfo.LastCount = trades.Count();
            symbolInfo.LastIntervalMin = interval.Days * 1440 + interval.Hours * 60 + interval.Minutes;

            await _request.TradeCoordinatorUpdateSymbolAsync(symbolInfo);

            _logger.LogInformation(
                "Летна. Импорт данных {0}-{1}, дата нач: {2}, дата кон: {3}, min: {4}, max: {5}, id: {6}, count: {7}",
                _serviceInfo.ExchangeName,
                _currencyPair,
                parameters.TimeStart.UpToSecondsToString(),
                parameters.TimeEnd.UpToSecondsToString(),
                dateMin.UpToSecondsToString(),
                dateMax.UpToSecondsToString(),
                id,
                trades.Count());

            return new ResultPerformance { Successful = true };
        }

        private async Task<ParametersTrade> GetParameters()
        {
            var tradeInfo = await _request.GetTradeInfoAsync(_serviceInfo.ExchangeName, _currencyPair);
            var symbolInfo = await _request.GetTradeCoordinatorSymbolAsync(_serviceInfo.ExchangeName, _currencyPair);
            var dateStart = DateTime.MinValue;

            if (tradeInfo.TimestampMax > 0)
            {
                dateStart = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(tradeInfo.TimestampMax).AddSeconds(-1);
            }
            else
            {
                if (symbolInfo != null && symbolInfo.TimestampInitial > 0)
                {
                    dateStart = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(symbolInfo.TimestampInitial);
                }
                else
                {
                    var exchangeInfo = await _request.GetTradeExchangeInfoAsync(_serviceInfo.ExchangeName);
                    if (exchangeInfo != null && exchangeInfo.TimestampInitial > 0)
                    {
                        dateStart = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(exchangeInfo.TimestampInitial);
                    }
                }
            }

            if (dateStart == DateTime.MinValue)
            {
                dateStart = TimeProvider.Current.UtcNow;
            }

            var dateEnd = dateStart.AddMinutes(30);
            if (dateEnd < TimeProvider.Current.UtcNow.AddMinutes(10) && symbolInfo != null &&
                symbolInfo.LastCount < 900 && symbolInfo.LastIntervalMin > 0)
            {
                dateEnd = dateStart.AddMinutes(symbolInfo.LastIntervalMin * 2);
            }

            var сurrent = TimeProvider.Current.UtcNow;
            if (dateEnd > сurrent)
            {
                dateEnd = new DateTime(сurrent.Year, сurrent.Month, сurrent.Day, сurrent.Hour, сurrent.Minute, 0,
                    DateTimeKind.Utc).AddMinutes(1);
            }

            if (dateEnd == dateStart)
            {
                dateEnd = dateStart.AddMinutes(1);
            }

            return new ParametersTrade
                { Symbol = _currencyPair, TimeStart = dateStart, TimeEnd = dateEnd, SymbolInfo = symbolInfo };
        }
    }
}
