namespace TradesStorage.Infrastructure.Dto
{
    public class Trade
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Штамп времени в миллисекундах
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Количество.Если больше 0 - покупка, меньше 0 - продажа.
        /// </summary>
        public double Amount { get; set; }
    }
}
