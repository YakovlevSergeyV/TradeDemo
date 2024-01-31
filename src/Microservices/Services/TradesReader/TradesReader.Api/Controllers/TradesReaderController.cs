namespace TradesReader.Api.Controllers
{
    using System.Net;
    using GlobalSpace.Common.Guardly;
    using Microsoft.AspNetCore.Mvc;
    using ServiceWorker.Abstract;
    using TradesReader.Api.Dto;
    using TradesReader.Api.Infrastructure.Services;

    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TradesReaderController : Controller
    {
        private readonly IServiceModelFacade _serviceModelFacade;
        private readonly ISymbolDictionary _symbolDictionary;

        public TradesReaderController
        (
            IServiceModelFacade serviceModelFacade,
            ISymbolDictionary symbolDictionary
        )
        {
            Guard.Argument(() => serviceModelFacade, Is.NotNull);
            
            _serviceModelFacade = serviceModelFacade;
            _symbolDictionary = symbolDictionary;
        }

        [Route("start")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult GetStart()
        {
            _serviceModelFacade.Start();
            return NoContent();
        }

        [Route("stop")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult GetStop()
        {
            _serviceModelFacade.Stop();
            return NoContent();
        }

        /// <summary>
        /// Состояние. True - происходит чтение ленты с биржы, False - нет чтения данных с биржы
        /// </summary>
        [Route("Running/{exchange}/{currencyPair}")]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public IActionResult GetRunning(string exchange, string currencyPair)
        {
            if (string.IsNullOrEmpty(exchange) || string.IsNullOrEmpty(currencyPair))
            {
                return BadRequest();
            }

            var symbol = new Symbol
            {
                ExchangeName = exchange.ToUpper(),
                CurrencyPairName = currencyPair.ToUpper()
            };

            return Ok(_symbolDictionary.Exists(symbol));
        }
    }
}
