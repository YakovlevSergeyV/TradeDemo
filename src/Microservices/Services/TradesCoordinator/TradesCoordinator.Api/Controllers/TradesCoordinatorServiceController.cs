namespace TradesCoordinator.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using GlobalSpace.Common.Guardly;
    using Microsoft.AspNetCore.Mvc;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.Synchronization;

    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TradesCoordinatorServiceController : Controller
    {
        private readonly ISynchronizationManager _synchronizationManager;

        public TradesCoordinatorServiceController(ISynchronizationManager synchronizationManager)
        {
            Guard.Argument(() => synchronizationManager, Is.NotNull);

            _synchronizationManager = synchronizationManager;
        }
        /// <summary>
        /// Возвращет список зарегистрированных сервисов
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Service>), (int)HttpStatusCode.OK)]
        public IActionResult GetAllServices()
        {
            var items = _synchronizationManager.Services;
            return Ok(items);
        }

        /// <summary>
        /// Возвращет признак запущен сервис или нет
        /// </summary>
        /// <param name="service">Данные описывающие сервис</param>
        /// <returns></returns>
        [Route("started")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(bool), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetStartedService([FromBody] Service service)
        {
            if (service == null
                || string.IsNullOrEmpty(service.Guid)
                || string.IsNullOrEmpty(service.HostName)
                || string.IsNullOrEmpty(service.ExchangeName))
            {
                return BadRequest();
            }

            service.HostName = service.HostName.ToUpper();
            service.ExchangeName = service.ExchangeName.ToUpper();

            var items = await _synchronizationManager.StartedService(service);
            return Ok(items);
        }

        /// <summary>
        /// Регистрация нового сервиса
        /// </summary>
        [Route("")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RegistrationService([FromBody] Service service)
        {
            if (service == null 
                || string.IsNullOrEmpty(service.Guid)
                || string.IsNullOrEmpty(service.HostName) 
                || string.IsNullOrEmpty(service.ExchangeName))
            {
                return BadRequest();
            }

            service.HostName = service.HostName.ToUpper();
            service.ExchangeName = service.ExchangeName.ToUpper();

            await _synchronizationManager.RegistrationService(service);

            return NoContent();
        }

        /// <summary>
        /// Сервис отработал успешно
        /// </summary>
        /// <param name="request">Данные описывающие результат успешного выполнения сервиса</param>
        /// <returns></returns>
        [Route("performance/successful")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> PerformanceSuccessful([FromBody] PerformanceSuccessful request)
        {
            if (request == null
                || string.IsNullOrEmpty(request.ExchangeName)
                || string.IsNullOrEmpty(request.CurrencyPairName)
                || request.Timestamp == 0)
            {
                return BadRequest();
            }

            request.HostName = request.HostName.ToUpper();
            request.ExchangeName = request.ExchangeName.ToUpper();
            request.CurrencyPairName = request.CurrencyPairName.ToUpper();

            await _synchronizationManager.PerformanceSuccessful(request);

            return NoContent();
        }

        /// <summary>
        /// Сервис отработал не успешно
        /// </summary>
        /// <param name="request">Данные описывающие результат не успешного выполнения сервиса</param>
        /// <returns></returns>
        [Route("performance/notsuccessful")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> PerformanceNotSuccessful([FromBody] PerformanceNotSuccessful request)
        {
            if (request == null
                || string.IsNullOrEmpty(request.ExchangeName)
                || string.IsNullOrEmpty(request.CurrencyPairName)
                || string.IsNullOrEmpty(request.Exception))
            {
                return BadRequest();
            }

            request.HostName = request.HostName.ToUpper();
            request.ExchangeName = request.ExchangeName.ToUpper();
            request.CurrencyPairName = request.CurrencyPairName.ToUpper();

            await _synchronizationManager.PerformanceNotSuccessful(request);
            return NoContent();
        }
    }
}
