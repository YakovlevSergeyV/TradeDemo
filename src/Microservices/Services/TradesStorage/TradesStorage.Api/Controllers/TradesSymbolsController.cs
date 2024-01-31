
namespace TradesStorage.Api.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using GlobalSpace.Common.Guardly;
    using Microsoft.AspNetCore.Mvc;
    using TradesStorage.Api.Infrastructure.Repositories;
    using TradesStorage.Api.Infrastructure.Services;
    using TradesStorage.Infrastructure.Dto;

    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TradesSymbolsController : Controller
    {
        private readonly ISymbolTradeService symbolTradeService;
        private readonly ISymbolTradeRepository symbolTradeRepository;
        private readonly ISymbolDictionary symbolDictionary;

        public TradesSymbolsController(
            ISymbolTradeService symbolTradeService,
            ISymbolTradeRepository symbolTradeRepository,
            ISymbolDictionary symbolDictionary)
        {
            Guard.Argument(() => symbolTradeService, Is.NotNull);
            Guard.Argument(() => symbolTradeRepository, Is.NotNull);

            this.symbolTradeService = symbolTradeService;
            this.symbolTradeRepository = symbolTradeRepository;
            this.symbolDictionary = symbolDictionary;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Symbol>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCurrencyPairTrade()
        {
            var items = await symbolTradeRepository.GetAllCurrencyPairTradeAsync();
            return Ok(items);
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> CreateCurrencyPairTrade([FromBody] Symbol symbol)
        {
            if (symbol == null
                || string.IsNullOrEmpty(symbol.ExchangeName)
                || string.IsNullOrEmpty(symbol.CurrencyPairName))
            {
                return BadRequest();
            }

            symbol.CurrencyPairName = symbol.CurrencyPairName.ToUpper();
            symbol.ExchangeName = symbol.ExchangeName.ToUpper();

            var exists = await symbolTradeRepository.ExistsCurrencyPairTradeAsync(symbol);
            if (!exists)
            {
                await symbolTradeService.CreateCurrencyPairTradeAsync(symbol);
            }

            return NoContent();
        }

        /// <summary>
        /// Удаление информации по торгам по вылютной паре из биржы
        /// </summary>
        [Route("")]
        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteCurrencyPairTrade([FromBody] Symbol symbol)
        {
            if (symbol == null
                || string.IsNullOrEmpty(symbol.ExchangeName)
                || string.IsNullOrEmpty(symbol.CurrencyPairName))
            {
                return BadRequest();
            }

            symbol.CurrencyPairName = symbol.CurrencyPairName.ToUpper();
            symbol.ExchangeName = symbol.ExchangeName.ToUpper();

            await symbolTradeService.DeleteCurrencyPairTradeAsync(symbol);
            return NoContent();
        }

        [Route("Length/{exchange}/{currencyPair}")]
        [HttpGet]
        [ProducesResponseType(typeof(long), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDatabaseLength(string exchange, string currencyPair)
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

            var length = await symbolTradeRepository.GetDatabaseLength(symbol);
            return Ok(length);
        }

        /// <summary>
        /// Состояние, включена или выключена валютная пара
        /// </summary>
        [Route("Running/{exchange}/{currencyPair}")]
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int) HttpStatusCode.OK)]
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

            return Ok(!symbolDictionary.Exists(symbol));
        }

        /// <summary>
        /// Включить валютную пару 
        /// </summary>
        [Route("switchin")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchIn(Symbol symbol)
        {
            if (symbol == null
                || string.IsNullOrEmpty(symbol.ExchangeName)
                || string.IsNullOrEmpty(symbol.CurrencyPairName))
            {
                return BadRequest();
            }

            symbol.CurrencyPairName = symbol.CurrencyPairName.ToUpper();
            symbol.ExchangeName = symbol.ExchangeName.ToUpper();

            symbolDictionary.DeleteSymbol(symbol);
            return NoContent();
        }

        /// <summary>
        /// Выключить валютную пару 
        /// </summary>
        [Route("switchoff")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchOff(Symbol symbol)
        {
            if (symbol == null
                || string.IsNullOrEmpty(symbol.ExchangeName)
                || string.IsNullOrEmpty(symbol.CurrencyPairName))
            {
                return BadRequest();
            }

            symbol.CurrencyPairName = symbol.CurrencyPairName.ToUpper();
            symbol.ExchangeName = symbol.ExchangeName.ToUpper();

            symbolDictionary.AddSymbol(symbol);
           
            return NoContent();
        }
    }
}
