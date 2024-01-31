namespace TradesReader.Api.Dto
{
    internal class Service
    {
        /// <summary>
        /// Guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Название компьютера
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Название биржы
        /// </summary>
        public string ExchangeName { get; set; }
    }
}
