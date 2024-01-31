namespace TradesCoordinator.Api.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using GlobalSpace.Common.Guardly;
    using Microsoft.AspNetCore.Mvc;
    using TradesCoordinator.Api.Services;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.Repositories;
    using TradesCoordinator.Infrastructure.Synchronization;

    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TradesCoordinatorSymbolsController : Controller
    {
        private readonly ICurrencyPairService _currencyPairService;
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly ISynchronizationManager _synchronizationManager;

        public TradesCoordinatorSymbolsController(
            ICurrencyPairService currencyPairService,
            ICurrencyPairRepository currencyPairRepository,
            ISynchronizationManager synchronizationManager)
        {
            Guard.Argument(() => currencyPairService, Is.NotNull);
            Guard.Argument(() => currencyPairRepository, Is.NotNull);
            Guard.Argument(() => synchronizationManager, Is.NotNull);

            _currencyPairService = currencyPairService;
            _currencyPairRepository = currencyPairRepository;
            _synchronizationManager = synchronizationManager;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SymbolInfo>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSymbol()
        {
            var items = await _currencyPairRepository.GetAllCurrencyPairAsync();
            return Ok(items);
        }

        [Route("{exchange}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SymbolInfo>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSymbols(string exchange)
        {
            if (string.IsNullOrEmpty(exchange))
            {
                return BadRequest();
            }

            exchange = exchange.ToUpper();
            var items = await _currencyPairRepository.GetCurrencyPairsAsync(exchange);
            return Ok(items);
        }


        [Route("{exchange}/{symbol}")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(SymbolInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSymbol(string exchange, string symbol)
        {
            if (string.IsNullOrEmpty(exchange)
                || string.IsNullOrEmpty(symbol))
            {
                return BadRequest();
            }

            exchange = exchange.ToUpper();
            symbol = symbol.ToUpper();

            var items = await _currencyPairRepository.GetCurrencyPairAsync(exchange, symbol);
            return Ok(items);
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> CreateSymbol([FromBody] SymbolInfo symbolInfo)
        {
            if (symbolInfo == null
                || string.IsNullOrEmpty(symbolInfo.ExchangeName)
                || string.IsNullOrEmpty(symbolInfo.CurrencyPairName))
            {
                return BadRequest();
            }

            symbolInfo.CurrencyPairName = symbolInfo.CurrencyPairName.ToUpper();
            symbolInfo.ExchangeName = symbolInfo.ExchangeName.ToUpper();

            var exists = await _currencyPairRepository.ExistsCurrencyPairAsync(symbolInfo);
            if (!exists)
            {
                await _currencyPairService.CreateCurrencyPairAsync(symbolInfo);
            }
            return NoContent();
        }

        [Route("")]
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateSymbol([FromBody] SymbolInfo symbolInfo)
        {
            if (symbolInfo == null
                || string.IsNullOrEmpty(symbolInfo.Guid)
                || string.IsNullOrEmpty(symbolInfo.ExchangeName)
                || string.IsNullOrEmpty(symbolInfo.CurrencyPairName))
            {
                return BadRequest();
            }

            symbolInfo.CurrencyPairName = symbolInfo.CurrencyPairName.ToUpper();
            symbolInfo.ExchangeName = symbolInfo.ExchangeName.ToUpper();

            var exists = await _currencyPairRepository.ExistsCurrencyPairAsync(symbolInfo.Guid);
            if (!exists)
            {
                return NotFound();
            }

            await _currencyPairService.UpdateCurrencyPairAsync(symbolInfo);
            return NoContent();
        }

        [Route("")]
        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteSymbol([FromBody] SymbolInfo symbolInfo)
        {
            if (symbolInfo == null
                || string.IsNullOrEmpty(symbolInfo.ExchangeName)
                || string.IsNullOrEmpty(symbolInfo.CurrencyPairName))
            {
                return BadRequest();
            }

            symbolInfo.CurrencyPairName = symbolInfo.CurrencyPairName.ToUpper();
            symbolInfo.ExchangeName = symbolInfo.ExchangeName.ToUpper();

            var exists = await _currencyPairRepository.ExistsCurrencyPairAsync(symbolInfo.Guid);
            if (!exists)
            {
                return NotFound();
            }

            await _currencyPairService.DeleteCurrencyPairAsync(symbolInfo);
            return NoContent();
        }

        [Route("nextSymbol")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(SymbolNext), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetNextSymbol([FromBody] Service service)
        {
            if (service == null
                || string.IsNullOrEmpty(service.HostName)
                || string.IsNullOrEmpty(service.ExchangeName))
            {
                return BadRequest();
            }

            service.HostName = service.HostName.ToUpper();
            service.ExchangeName = service.ExchangeName.ToUpper();

            var items = await _synchronizationManager.NextCurrencyPair(service);
            return Ok(items);
        }

        /// <summary>
        /// Состояние, включена или выключена валютная пара
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

            return Ok(_synchronizationManager.GetRunning(symbol));
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
  
            _synchronizationManager.SwitchIn(symbol);
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

            _synchronizationManager.SwitchOff(symbol);
            return NoContent();
        }
    }
}
