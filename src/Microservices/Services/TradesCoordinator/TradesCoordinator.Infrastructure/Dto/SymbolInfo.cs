namespace TradesCoordinator.Infrastructure.Dto
{
    public class SymbolInfo
    {
        public string Guid { get; set; }
        public string CurrencyPairName { get; set; }
        public string ExchangeName { get; set; }
        public long TimestampInitial { get; set; }

        /// <summary>
        /// Количество заказов прочитанных за последний запрос к бирже
        /// </summary>
        public int LastCount { get; set; }

        /// <summary>
        /// Интервал в минутах, между начальной и конечной датой, используемый в последнем запросе к бирже чтения заказов
        /// </summary>
        public int LastIntervalMin { get; set; }
    }
}
