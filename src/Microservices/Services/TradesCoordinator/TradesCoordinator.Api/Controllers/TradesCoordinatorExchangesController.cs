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

    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TradesCoordinatorExchangesController : Controller
    {
        private readonly IExchangeService _exchangeService;
        private readonly IExchangeRepository _exchangeRepository;

        public TradesCoordinatorExchangesController(
            IExchangeRepository exchangeRepository
            , IExchangeService exchangeService)
        {
            Guard.Argument(() => exchangeRepository, Is.NotNull);
            Guard.Argument(() => exchangeService, Is.NotNull);

            _exchangeRepository = exchangeRepository;
            _exchangeService = exchangeService;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExchangeInfo>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllExchange()
        {
            var items = await _exchangeRepository.GetAllExchangeAsync();
            return Ok(items);
        }

        [Route("{exchange}")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ExchangeInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExchange(string exchange)
        {
            if (string.IsNullOrEmpty(exchange))
            {
                return BadRequest();
            }

            exchange = exchange.ToUpper();

            var items = await _exchangeRepository.GetExchangeByNameAsync(exchange);
            return Ok(items);
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> CreateExchange([FromBody] ExchangeInfo exchange)
        {
            if (exchange == null
                || string.IsNullOrEmpty(exchange.ExchangeName))
            {
                return BadRequest();
            }

            exchange.ExchangeName = exchange.ExchangeName.ToUpper();

            var exists = await _exchangeRepository.ExistsExchangeAsync(exchange);
            if (!exists)
            {
                await _exchangeService.CreateExchangeAsync(exchange);
            }
            return NoContent();
        }

        [Route("")]
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateExchange([FromBody] ExchangeInfo exchange)
        {
            if (exchange == null
                || string.IsNullOrEmpty(exchange.Guid)
                || string.IsNullOrEmpty(exchange.ExchangeName))
            {
                return BadRequest();
            }
            
            exchange.ExchangeName = exchange.ExchangeName.ToUpper();

            var exists = await _exchangeRepository.ExistsExchangeAsync(exchange.Guid);
            if (!exists)
            {
                return NotFound();
            }

            await _exchangeService.UpdateExchangeAsync(exchange);
            return NoContent();
        }

        [Route("")]
        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteExchange([FromBody] ExchangeInfo exchange)
        {
            if (exchange == null
                || string.IsNullOrEmpty(exchange.Guid)
                || string.IsNullOrEmpty(exchange.ExchangeName))
            {
                return BadRequest();
            }

            exchange.ExchangeName = exchange.ExchangeName.ToUpper();

            var exists = await _exchangeRepository.ExistsExchangeAsync(exchange.Guid);
            if (!exists)
            {
                return NotFound();
            }

            await _exchangeService.DeleteExchangeAsync(exchange);
            return NoContent();
        }
    }
}
