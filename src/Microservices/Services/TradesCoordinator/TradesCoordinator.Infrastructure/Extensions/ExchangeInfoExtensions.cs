namespace TradesCoordinator.Infrastructure.Extensions
{
    using System;
    using TradesCoordinator.Infrastructure.Dto;

    public static class ExchangeInfoExtensions
    {
        public static void Update(this ExchangeInfo exchangeInfo, ExchangeInfo exchange)
        {
            if (exchange == null)
            {
                throw new ArgumentNullException(nameof(exchange));
            }

            exchangeInfo.HeartBeatCycleInMs = exchange.HeartBeatCycleInMs;
            exchangeInfo.TimestampInitial = exchange.TimestampInitial;
        }
    }
}
