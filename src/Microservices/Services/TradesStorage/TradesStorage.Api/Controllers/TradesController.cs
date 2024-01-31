
namespace TradesStorage.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using GlobalSpace.Common.Guardly;
    using Microsoft.AspNetCore.Mvc;
    using TradesStorage.Api.Infrastructure.Repositories;
    using TradesStorage.Api.Infrastructure.Services;
    using TradesStorage.Infrastructure.Dto;
    using TradesStorage.Infrastructure.EntityContext;
    using TradesStorage.Infrastructure.Repositories;
    using TradesStorage.Infrastructure.UpgradeDb;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class TradesController : Controller
    {
        private readonly ISymbolTradeRepository symbolTradeRepository;
        private readonly Func<Symbol, IProviderTradeContext> providerTradeContextFactory;
        private readonly Func<Symbol, TradeContext, ITradeRepository> tradeRepositoryFactory;
        private readonly
            Func<Symbol, Func<Symbol, IProviderTradeContext>, Func<Symbol, TradeContext, ITradeRepository>, ITradeService>
            tradeServiceFactory;

        public TradesController
        (
            Func<Symbol, IProviderTradeContext> providerTradeContextFactory
            , Func<Symbol, TradeContext, ITradeRepository> tradeRepositoryFactory
            , Func<Symbol, Func<Symbol, IProviderTradeContext>, Func<Symbol, TradeContext, ITradeRepository>, ITradeService> tradeServiceFactory
            , ISymbolTradeRepository symbolTradeRepository)
        {
            Guard.Argument(() => providerTradeContextFactory, Is.NotNull);
            Guard.Argument(() => tradeServiceFactory, Is.NotNull);
            Guard.Argument(() => tradeRepositoryFactory, Is.NotNull);
            Guard.Argument(() => symbolTradeRepository, Is.NotNull);

            this.providerTradeContextFactory = providerTradeContextFactory;
            this.tradeServiceFactory = tradeServiceFactory;
            this.tradeRepositoryFactory = tradeRepositoryFactory;
            this.symbolTradeRepository = symbolTradeRepository;
        }

        [Route("info/{exchange}/{currencyPair}")]
        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TradeInfo), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTradeInfo(string exchange, string currencyPair)
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

            var exists = await symbolTradeRepository.ExistsCurrencyPairTradeAsync(symbol);
            if (!exists)
            {
                return NotFound();
            }


            var tradeRepository = GetTradeRepository(symbol);
            var model = await tradeRepository.GetInfoAsync();

            return Ok(model);
        }

        [Route("{exchange}/{currencyPair}/{timestampMin:long}/{timestampMax:long}")]
        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Trade>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTrades(string exchange, string currencyPair,
            long timestampMin, long timestampMax)
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

            var exists = await symbolTradeRepository.ExistsCurrencyPairTradeAsync(symbol);
            if (!exists)
            {
                return NotFound();
            }

            var tradeRepository = GetTradeRepository(symbol);
            var items = await tradeRepository.GetAsync(timestampMin, timestampMax);
            return Ok(items);
        }

        [Route("update/{exchange}/{currencyPair}/{timestamp:long}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateTimestamp(string exchange, string currencyPair, long timestamp)
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

            var exists = await symbolTradeRepository.ExistsCurrencyPairTradeAsync(symbol);
            if (!exists)
            {
                return NotFound();
            }

            var tradeRepository = GetTradeRepository(symbol);
            await tradeRepository.UpdateTimestampAsync(timestamp);

            return NoContent();
        }

        [Route("insert/{exchange}/{currencyPair}/{timestamp:long}")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> InsertTrades(string exchange, string currencyPair, long timestamp,
            [FromBody] IEnumerable<Trade> trades)
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

            var exists = await symbolTradeRepository.ExistsCurrencyPairTradeAsync(symbol);
            if (!exists)
            {
                return NotFound();
            }


            var tradeService = GetTradeService(symbol);
            await tradeService.InsertAsync(trades, timestamp);

            return NoContent();
        }

        private ITradeRepository GetTradeRepository(Symbol symbol)
        {
            var provider = providerTradeContextFactory.Invoke(symbol);
            return tradeRepositoryFactory(symbol, provider.Context);
        }

        private ITradeService GetTradeService(Symbol symbol)
        {
            return tradeServiceFactory(symbol, providerTradeContextFactory, tradeRepositoryFactory);
        }
    }
}
