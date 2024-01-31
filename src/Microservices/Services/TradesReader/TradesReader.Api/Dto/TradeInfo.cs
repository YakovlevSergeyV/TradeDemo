namespace TradesReader.Api.Dto
{
    internal class TradeInfo
    {
        public string Guid { get; set; }

        /// <summary>
        /// Биржа
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// Вылютная пара
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Возвращает минимальный сохраненный штамп времени
        /// </summary>
        public long TimestampMin { get; set; }

        /// <summary>
        /// Возвращает максимальный сохраненный штамп времени
        /// </summary>
        public long TimestampMax { get; set; }
    }
}
